using System;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Configuration;

namespace api.Core.Mailer
{
    public class OverrideMailQueue : IQueueManager
    {
        public Task<bool> Send(MailMessage mMailMessage)
        {
            EmailUtils.SendEmail(mMailMessage, sentSuccesfully =>
                                                   {
                                                       if (!sentSuccesfully) return;
                                                       OnSent(mMailMessage);
                                                   });

            return Task.FromResult(true);
        }

        protected virtual void OnSent(MailMessage m)
        {
            if (Sent != null)
            {
                Sent(this, new MailMessageSentEventArgs { Message = m });
            }
        }
        
        public EventHandler<MailMessageSentEventArgs> Sent { get; set; }
        
        public void StopQueue()
        {
            // this is a no op
        }

        public void StartQueue()
        {
            // this is a no op
        }
    }
}