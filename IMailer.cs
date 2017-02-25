using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace api.Core.Mailer
{
    public interface IMailer<in T> where T: IQueueManager
    {
        EventHandler<MailMessageSentEventArgs> Sent { get; set; }
        Task<bool> Send(MailMessage mailMessage);
    }
}