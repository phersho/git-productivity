using System.Net.Mail;

namespace api.Core.Mailer
{
    public class MailMessageSentEventArgs
    {
        public MailMessage Message { get; set; }
    }
}