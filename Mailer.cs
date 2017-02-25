using System;
using System.Net.Mail;
using System.Threading.Tasks;
using NLog;

namespace api.Core.Mailer
{
    public class Mailer : IMailer<IQueueManager> 
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IQueueManager _queue;
        public EventHandler<MailMessageSentEventArgs> Sent { get; set; }

        public Mailer(IQueueManager senderManager)
        {
            _queue = senderManager;
        }
        
        public Task<bool> Send(MailMessage mailMessage)
        {
            _queue.Sent = Sent; // make sure the event is wired up
            
            var sendTask  = _queue.Send(mailMessage);
            sendTask.ContinueWith(state => _logger.Trace("Send task started"));
            return sendTask;
        }
    }
}
