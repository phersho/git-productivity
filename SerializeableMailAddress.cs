using System;
using System.Net.Mail;

namespace api.Core.Mailer
{
    [Serializable]
    public class SerializeableMailAddress
    {
        private readonly  String User;
        private readonly String Host;
        private readonly String Address;
        private readonly String DisplayName;

        public SerializeableMailAddress(MailAddress address)
        {
            User = address.User;
            Host = address.Host;
            Address = address.Address;
            DisplayName = address.DisplayName;
        }

        public MailAddress GetMailAddress()
        {
            return new MailAddress(Address, DisplayName);
        }
    }
}