using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Nito.AsyncEx;

namespace api.Core.Mailer
{
    public class DelayedMailQueue : IQueueManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly AsyncCollection<MailMessage> _queue;
        private readonly CancellationTokenSource _endProcessingToken;

        private readonly string _queueRecoveryFolder;
        private readonly int _sendDelay;

        public DelayedMailQueue(int delay)
        {
            _queue = new AsyncCollection<MailMessage>(new ConcurrentQueue<MailMessage>());
            _endProcessingToken = new CancellationTokenSource();

            _sendDelay = delay;

            _queueRecoveryFolder =
                //ConfigurationManager.AppSettings["delayedqueue.recovery"] ?? AppDomain.CurrentDomain.BaseDirectory;
                ConfigurationManager.AppSettings["delayedqueue.recovery"] ?? @"C:\temp\email";

            StartQueue();
        }

        public void StopQueue()
        {
            _endProcessingToken.Cancel(); // cancel concurrent processing     
            _queue.CompleteAddingAsync();
        }

        protected virtual void OnSent(MailMessage m)
        {
            if (Sent != null)
            {
                Sent(this, new MailMessageSentEventArgs { Message = m});
            }    
        }

        private async Task<MailMessage> ReadFromFile(string filename)
        {
            using (var sourceStream = new FileStream(filename,
                                                     FileMode.Open, FileAccess.Read, FileShare.Read,
                                                     bufferSize: 0x4000, useAsync: true))
            using (var memoryStream = new MemoryStream(0x4000))
            {
                var buffer = new byte[0x4000];

                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    memoryStream.Write(buffer, 0, numRead);
                }
                
                try
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var formatter = new BinaryFormatter();
                    var msg = formatter.Deserialize(memoryStream) as SerializeableMailMessage;

                    if (msg == null) throw new SerializationException("Could not serialize mailmessage from disk");

                    return msg.GetMailMessage();
                }
                catch (Exception e)
                {
                    _logger.WarnException("Could not read the MailMessage from disk - processing will continue.", e);
                    return new MailMessage();
                }
            }
        }


        public void StartQueue()
        {
            RestoreMessagesFromDisk();

            Task.Run(async () =>
                               {
                                   while (!_endProcessingToken.IsCancellationRequested)
                                   {
                                       var m =
                                           await _queue.TryTakeAsync(_endProcessingToken.Token);

                                       if (m.Success && m.Collection != null && m.Item != null)
                                       {
                                           OnSend(m.Item); // this is purposedly NOT awaited. 
                                       }
                                   }

                                   _logger.Info("DelayedQueueManager has stopped.");
                               });
        }

        private void RestoreMessagesFromDisk()
        {
            // try to populate the colleciton with values from disk, if any
            var now = DateTime.UtcNow;
            var messages =
                from filename in Directory.GetFiles(_queueRecoveryFolder, "*.delayedqueue").AsParallel()
                let lastModified = File.GetLastAccessTimeUtc(filename)
                where now.Subtract(lastModified).TotalMinutes < 60
                select new Tuple<string, Task<MailMessage>>(filename, ReadFromFile(filename));

            messages.ForEachAsync(async m =>
                                            {
                                                if (await _queue.TryAddAsync(await m.Item2)) File.Delete(m.Item1);
                                            });

        }

        // tis method is void - fire and forget!
        private async void OnSend(MailMessage mMailMessage)
        {
            string filename = await SaveEmail(mMailMessage);
            bool cancelRequest = false;

            try
            {
                await Task.Delay(_sendDelay, _endProcessingToken.Token);
            }
            catch (TaskCanceledException e)
            {
                _logger.Trace("A request to cancel message send received... {0}", mMailMessage.Subject);
                cancelRequest = true;
            }

            if (cancelRequest) return;

            EmailUtils.SendEmail(mMailMessage, sentSuccesfully =>
                                                   {
                                                       if (!sentSuccesfully) return;

                                                       try
                                                       {
                                                           File.Delete(filename);
                                                           OnSent(mMailMessage);
                                                       }
                                                       catch (Exception e)
                                                       {
                                                           _logger.FatalException(
                                                               "File could not be removed from queue",
                                                               e);
                                                       }

                                                       _logger.Trace(
                                                           "Processed - delayed email sent. {0}",
                                                           filename);
                                                   });


        }

        private Task<string> SaveEmail(MailMessage mMailMessage)
        {
            return Task.Run(async () =>
                                      {
                                           
                                          var filename = string.Format("{0}.delayedqueue", Guid.NewGuid());
                                          filename = Path.Combine(_queueRecoveryFolder, filename);                                         

                                          await WriteToFile(mMailMessage, filename);

                                          return filename;
                                      });
        }

        private static async Task WriteToFile(MailMessage mMailMessage, string filename)
        {
            using (var memoryStream = new MemoryStream())
            using (var fs = new FileStream(filename, FileMode.Create,
                                           FileAccess.ReadWrite, FileShare.ReadWrite,
                                           0x4000, true))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, new SerializeableMailMessage(mMailMessage));
                memoryStream.Seek(0, SeekOrigin.Begin);

                await memoryStream.CopyToAsync(fs);

                // we do this to make sure everything is on the disk (or in the OS hands, really)
                await fs.FlushAsync();
            }
        }

        public Task<bool> Send(MailMessage mailMessage)
        {
            return _queue.TryAddAsync(mailMessage);
        }

        public EventHandler<MailMessageSentEventArgs> Sent { get; set; }
    }
}