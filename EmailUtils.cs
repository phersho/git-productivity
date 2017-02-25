using System;
using System.Net.Mail;
using NLog;
using models;


namespace api.Core.Mailer
{
    static internal class EmailUtils
    {
        //public enum domains{
        //    Gmail=0,
        //    Verizon=1,
        //    Hotmail=2,
        //    ATyT=3,
        //    DefinityFirst=4
        //}

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public static void SendEmail(MailMessage mMailMessage, Action<bool> completed)
        {            
            try
            {
                _logger.Debug("EmailUtils.SendEmail");
                EmailManager.EmailManager objEmailManager = new EmailManager.EmailManager();

                EmailManager.Model.MailMessageFullInfo mailMessageFullInfo = new EmailManager.Model.MailMessageFullInfo();
                mailMessageFullInfo.MmMessage = mMailMessage;

                MailMessageFullInfo mmFullInfo = ((models.MailMessageFullInfo)(mMailMessage));
                mailMessageFullInfo.deliveryMethod = mmFullInfo.deliveryMethod;
                mailMessageFullInfo.Pager = mmFullInfo.Pager;
                mailMessageFullInfo.TextMessage = mmFullInfo.TextMessage;
                mailMessageFullInfo.Email = mmFullInfo.Email;
                mailMessageFullInfo.PushNotification = mmFullInfo.PushNotification;
                mailMessageFullInfo.messageType = (EmailManager.EmailManager.MessageType)mmFullInfo.messageType;
                //EmailManager.EmailManager.SendEMail(mailMessageFullInfo, b => { });
                objEmailManager.SendEMail(mailMessageFullInfo, b => { });
            }
            catch (Exception EX)
            {
                _logger.Debug("EmailUtils.SendEmail" + EX.Message);                
            }            
        }
    }
}