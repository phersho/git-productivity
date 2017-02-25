using System;
using System.Configuration;
using Blitz.Commissions.Core.Infrastructure;
using Blitz.Core.ViewModels.Configuration;

namespace Blitz.Core.Infrastructure
{
    public class Utilities
    {
        private Utilities()
        {

        }

        private static Utilities _Util;

        public static Utilities Instance
        {
            get
            {
                if (_Util == null)
                    _Util = new Utilities();

                return _Util;
            }
        }

        public ApplicationOptions GetConfiguration()
        {
            return new ApplicationOptions
            {
                DefaultTenant = ConfigurationManager.AppSettings["DefaultTenant"],
                ApplicationWwwRoot = ConfigurationManager.AppSettings["DefaultTenant"],
                SessionTimeOut = Convert.ToDouble(ConfigurationManager.AppSettings["SessionTimeOut"]),
                MongoConnectionString = ConfigurationManager.ConnectionStrings["Mongo"].ConnectionString,
                MongoDB = ConfigurationManager.AppSettings["MongoDatabase"],
                URLTemplate = ConfigurationManager.AppSettings["URLTemplate"],
                AuthenticationCookieDomain = ConfigurationManager.AppSettings["Authentication:CookieDomain"],
                ApplicationPort = ConfigurationManager.AppSettings["Application:Port"],
                MarketingEmail = ConfigurationManager.AppSettings["MarketingEmail"],
                ExcelTemplateURL = ConfigurationManager.AppSettings["ExcelTemplateURL"],

                RequestTokenUrl = ConfigurationManager.AppSettings["RequestTokenUrl"],
                OauthUrl = ConfigurationManager.AppSettings["OauthUrl"],
                AccessTokenUrl = ConfigurationManager.AppSettings["AccessTokenUrl"],
                UserAuth = ConfigurationManager.AppSettings["UserAuth"],
                OauthCBUrl = ConfigurationManager.AppSettings["OauthCBUrl"],
                ProdUrl = ConfigurationManager.AppSettings["ProdUrl"],
                Quickbooks_TxnDate = ConfigurationManager.AppSettings["Quickbooks_TxnDate"],
                ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"],
                AppId = ConfigurationManager.AppSettings["AppId"],
                AppUrl = ConfigurationManager.AppSettings["AppUrl"],
                AppSupport = ConfigurationManager.AppSettings["AppSupport"],
                OwnerId = ConfigurationManager.AppSettings["OwnerId"],
                RunEveryMinutes = ConfigurationManager.AppSettings["RunEveryMinutes"],

                XERO_ConsumerKey = ConfigurationManager.AppSettings["XERO_ConsumerKey"],
                XERO_ConsumerSecret = ConfigurationManager.AppSettings["XERO_ConsumerSecret"],
                XERO_API_RequestTokenUrl = ConfigurationManager.AppSettings["XERO_API_RequestTokenUrl"],
                XERO_API_AuthorizeUrl = ConfigurationManager.AppSettings["XERO_API_AuthorizeUrl"],
                XERO_API_AccessTokenUrl = ConfigurationManager.AppSettings["XERO_API_AccessTokenUrl"],
                XERO_API_Base = ConfigurationManager.AppSettings["XERO_API_Base"],
                XERO_AuthCallbackUrl = ConfigurationManager.AppSettings["XERO_AuthCallbackUrl"],

                SignNowClientId = ConfigurationManager.AppSettings["SignNowClientId"],
                SignNowClientSecret = ConfigurationManager.AppSettings["SignNowClientSecret"],
                SignNowAuthorizeRedirectUrl = ConfigurationManager.AppSettings["SignNowAuthorizeRedirectUrl"],
                SignNowApiBaseUrl = ConfigurationManager.AppSettings["SignNowApiBaseUrl"],

                BillDotCom_API_Base = ConfigurationManager.AppSettings["BillDotCom_API_Base"],
                BillDotCom_API_SandBox = ConfigurationManager.AppSettings["BillDotCom_API_SandBox"],
                BillDotCom_API_Key = ConfigurationManager.AppSettings["BillDotCom_API_Key"]
            };
        }

        public MailerConfiguration GetMailerConfiguration()
        {
            return new MailerConfiguration
            {
                ClientURL = ConfigurationManager.AppSettings["Mailer-ClientURL"],
                ClientPort = Convert.ToInt32(ConfigurationManager.AppSettings["Mailer-ClientPort"]),
                MailAddress = ConfigurationManager.AppSettings["Mailer-MailAddress"],
                Username = ConfigurationManager.AppSettings["Mailer-Username"],
                Password = ConfigurationManager.AppSettings["Mailer-Password"]
            };
        }       
    }
}