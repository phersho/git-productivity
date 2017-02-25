namespace Blitz.Commissions.Core.Infrastructure
{
    public interface IApplicationOptions
    {
        string ConnectionString { get; set; }
        string DefaultTenant { get; set; }
        string ApplicationWwwRoot { get; set; }
        double SessionTimeOut { get; set; }
        string MongoConnectionString { get; set; }
        string MongoDB { get; set; }
        string URLTemplate { get; set; }
        string AuthenticationCookieDomain { get; set; }
        string ApplicationUrl { get; set; }
        string ApplicationPort { get; set; }
        string ApiUrl { get; set; }
        string MarketingEmail { get; set; }
        string ExcelTemplateURL { get; set; }
        string RequestTokenUrl { get; set; }
        string OauthUrl { get; set; }
        string AccessTokenUrl { get; set; }
        string UserAuth { get; set; }
        string OauthCBUrl { get; set; }
        string ProdUrl { get; set; }
        string Quickbooks_TxnDate { get; set; }
        string ConsumerKey { get; set; }
        string ConsumerSecret { get; set; }
        string AppId { get; set; }
        string AppUrl { get; set; }
        string AppSupport { get; set; }
        string OwnerId { get; set; }
        string RunEveryMinutes { get; set; }
        string XERO_ConsumerKey { get; set; }
        string XERO_ConsumerSecret { get; set; }
        string XERO_API_RequestTokenUrl { get; set; }
        string XERO_API_AuthorizeUrl { get; set; }
        string XERO_API_AccessTokenUrl { get; set; }
        string XERO_API_Base { get; set; }
        string XERO_AuthCallbackUrl { get; set; }
        string SignNowClientId { get; set; }
        string SignNowClientSecret { get; set; }
        string SignNowAuthorizeRedirectUrl { get; set; }
        string SignNowApiBaseUrl { get; set; }
        string SignNowInviteSubject { get; set; }
        string SignNowInviteMessage { get; set; }
        string BillDotCom_API_Base { get; set; }
        string BillDotCom_API_SandBox { get; set; }
        string BillDotCom_API_Key { get; set; }
    }

    public class ApplicationOptions : IApplicationOptions
    {
        public string ConnectionString { get; set; }
        public string DefaultTenant { get; set; }
        public string ApplicationWwwRoot { get; set; }
        public double SessionTimeOut { get; set; }
        public string MongoConnectionString { get; set; }
        public string MongoDB { get; set; }
        public string URLTemplate { get; set; }
        public string AuthenticationCookieDomain { get; set; }
        public string ApplicationUrl { get; set; }
        public string ApplicationPort { get; set; }
        public string ApiUrl { get; set; }
        public string MarketingEmail { get; set; }
        public string ExcelTemplateURL { get; set; }
        public string RequestTokenUrl { get; set; }
        public string OauthUrl { get; set; }
        public string AccessTokenUrl { get; set; }
        public string UserAuth { get; set; }
        public string OauthCBUrl { get; set; }
        public string ProdUrl { get; set; }
        public string Quickbooks_TxnDate { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AppId { get; set; }
        public string AppUrl { get; set; }
        public string AppSupport { get; set; }
        public string OwnerId { get; set; }
        public string RunEveryMinutes { get; set; }
        public string XERO_ConsumerKey { get; set; }
        public string XERO_ConsumerSecret { get; set; }
        public string XERO_API_RequestTokenUrl { get; set; }
        public string XERO_API_AuthorizeUrl { get; set; }
        public string XERO_API_AccessTokenUrl { get; set; }
        public string XERO_API_Base { get; set; }
        public string XERO_AuthCallbackUrl { get; set; }
        public string SignNowClientId { get; set; }
        public string SignNowClientSecret { get; set; }
        public string SignNowAuthorizeRedirectUrl { get; set; }
        public string SignNowApiBaseUrl { get; set; }
        public string SignNowInviteSubject { get; set; }
        public string SignNowInviteMessage { get; set; }
        public string BillDotCom_API_Base { get; set; }
        public string BillDotCom_API_SandBox { get; set; }
        public string BillDotCom_API_Key { get; set; }
    }
}