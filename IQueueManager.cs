using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace api.Core.Mailer
{
    public interface IQueueManager
    {
        Task<bool> Send(MailMessage mailMessage);
        EventHandler<MailMessageSentEventArgs> Sent { get; set; }
        void StopQueue();
        void StartQueue();
    }
}