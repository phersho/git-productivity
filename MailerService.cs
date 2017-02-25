using System;
using Blitz.Core.Infrastructure.Mailer.Templates.Immediately;
using System.Net.Mail;
using System.Text;
using Blitz.Core.Infrastructure.Logger.Services;
using Blitz.Core.ViewModels.Configuration;
using Blitz.Core.Model.Dynamic;
using System.Web;
using Blitz.Core.Infrastructure.Cache;
using MongoDB.Bson;
using Blitz.Core.Repository.Interface.Dynamic;
using Blitz.Core.Repository.Implementation.Dynamic;
using Blitz.Core.Infrastructure.Mailer.Templates.Daily;
using System.Collections.Generic;
using Blitz.Core.Enums;

namespace Blitz.Core.Infrastructure.Mailer.Services
{
    public class MailerService : IMailerService
    {
        private IBlitzDatabasePool _dbPool;
        private readonly ILogger _log;
        private ICache _cache;
        private Tenant _tenant;

        public MailerService(IBlitzDatabasePool dbPool, ILogger log, ICache cache)
        {
            _dbPool = dbPool;
            _log = log;
            _cache = cache;
        }

        public MailerService(Tenant tenant)
        {
            _tenant = tenant;
        }

        #region Immediately
        public string GetForgotPasswordBody(string email, string callbackUrl, string domainUrl)
        {
            return new ForgotPassword(email, callbackUrl, domainUrl).TransformText();
        }

        public string GetRequestAccessBody(string tenantName, string tenantEmail, string firstName, string lastName, string email, string domainUrl)
        {
            return new RequestAccess(tenantName, tenantEmail, firstName, lastName, email, domainUrl).TransformText();
        }

        public string GetPaidCommissionsSalesPersonBody(string salesPerson, string manager, Tenant tenant)
        {
            return new NotifyPaidCommissionToSalesperson(salesPerson, manager, tenant.Domain).TransformText();
        }

        public string GetRegisterBody(string name, string domainUrl, string userDomain)
        {
            return new Register(name, domainUrl, userDomain).TransformText();
        }

        public string GetNotifyMarketingBody(string firstName, string lastName, string email, string company, string phone, string domainUrl)
        {
            return new NotifyMarketing(firstName, lastName, email, company, phone, domainUrl).TransformText();
        }

        public string GetPayCommissionsSalesManagerBody(string salesManager, string transactionType, Tenant tenant)
        {
            return new RemindToPayCommissionsToSalesManager(salesManager, transactionType, tenant.Domain).TransformText();
        }

        public string GetTrialExpirationBody(string userFullName, int daysRemaining, string domainUrl)
        {
            return new SendTrialExpirationReminder(userFullName, daysRemaining, domainUrl).TransformText();
        }

        public string GetSignInNewDeviceBody(string userFullName, string callbackurl, string domainUrl)
        {
            return new SignInNewDevice(userFullName, callbackurl, domainUrl).TransformText();
        }

        public string GetSystemMaintenanceBody(DateTime startDate, DateTime endDate, string domainUrl, string dateFormat)
        {
            return new NotifySystemMaintenance(startDate, endDate, domainUrl, dateFormat).TransformText();
        }

        public string GetNewCompensationPlanBody(string salesPerson, string manager, Tenant tenant)
        {
            return new NotifyCompensationPlanCreated(salesPerson, manager, tenant.Domain).TransformText();
        }

        public string GetSalesTransactionCancelledBody(string salesPerson, string name, DateTime date, string transactionType, double amount, Tenant tenant, string dateFormat)
        {
            return new CancelSalesTransaction(salesPerson, name, date, transactionType, amount, tenant.Domain, dateFormat).TransformText();
        }

        public string GetUpdatedCompensationPlanBody(string salesPerson, Tenant tenant)
        {
            return new NotifyCompensationPlanUpdated(salesPerson, tenant.Domain).TransformText();
        }

        public string GetIndividualPaidSalesTransactionBody(string salesPerson, string manager, string name, string transactionType, double amount, Tenant tenant)
        {
            return new NotifyIndividualPaidSalesTransaction(salesPerson, manager, name, transactionType, amount, tenant.Domain).TransformText();
        }

        public string GetNewUserBody(string userFullName, string userId, Tenant tenant)
        {
            return new RegisterNewUser(userFullName, userId, tenant).TransformText();
        }

        public string SendUserInvitation(string firstName, string lastName, string email, string link, Tenant tenant)
        {
            return new SendUserInvitation(firstName, lastName, email, link, tenant.Domain, tenant.Name).TransformText();
        }
        #endregion

        #region Daily
        public string GetTransactionSummaryBody(List<Dictionary<string, object>> transactionsCountByStatus, List<DynamicRecord> statuses, string transactionType, string role, Tenant tenant)
        {
            return new SendTransactionSummary(transactionsCountByStatus, statuses, transactionType, role, tenant, Recurrence.Weekly).TransformText();
        }

        public string GetPaidTransactionListBody(List<Dictionary<string, object>> transactions, string transactionType, Tenant tenant)
        {
            return new SendPaidTransactionList(transactions, transactionType, tenant).TransformText();
        }

        public string GetForecastedCommissionsSalespersonBody(string name, string transactionType, Tenant tenant)
        {
            return new SendForecastedCommissionsToSalesperson(name, transactionType, tenant).TransformText();
        }

        public string GetForecastedCommissionsSalesManagerBody(List<Dictionary<string, object>> newInvoices, List<Dictionary<string, object>> updatedInvoices, string manager,
            string transactionType, Tenant tenant)
        {
            return new SendForecastedCommissionsToSalesManager(newInvoices, updatedInvoices, manager, transactionType, tenant).TransformText();
        }

        public string GetDueCommissionsSalespersonBody(List<Dictionary<string, object>> paidTransactions, string name, string transactionType, Tenant tenant)
        {
            return new SendDueCommissionsToSalesperson(paidTransactions, name, transactionType, tenant).TransformText();
        }

        public string GetDueCommissionsSalesManagerBody(List<Dictionary<string, object>> paidTransactions, string name, string transactionType, Tenant tenant)
        {
            return new SendDueCommissionsToSalesManager(paidTransactions, name, transactionType, tenant).TransformText();
        }
        #endregion
        
        #region Weekly
        public string GetWeeklyTransactionSummaryBody(List<Dictionary<string, object>> transactionsCountByStatus, List<DynamicRecord> statuses, string transactionType, string role, Tenant tenant)
        {
            return new SendTransactionSummary(transactionsCountByStatus, statuses, transactionType, role, tenant, Recurrence.Weekly).TransformText();
        }

        public string GetWeeklyPaidTransactionListBody(List<Dictionary<string, object>> transactions, string transactionType, Tenant tenant)
        {
            return new SendPaidTransactionList(transactions, transactionType, tenant).TransformText();
        }

        public string GetAdminSummaryBody(List<Dictionary<string, object>> lastWeekCommissions, List<Dictionary<string, object>> thisWeekCommissions,
            List<Dictionary<string, object>> lastWeekInvoices, List<Dictionary<string, object>> thisWeekInvoices,
            List<Dictionary<string, object>> lastWeekUserSales, List<Dictionary<string, object>> thisWeekUserSales,
            List<DynamicRecord> invoiceStatuses, List<DynamicRecord> incentiveStatuses, List<DynamicRecord> users,
            string transactionType, string name, Tenant tenant)
        {
            return new Templates.Weekly.SendSummaryReportToAdmin(lastWeekCommissions, thisWeekCommissions, lastWeekInvoices, thisWeekInvoices, lastWeekUserSales, thisWeekUserSales,
               invoiceStatuses, incentiveStatuses, users, transactionType, name, tenant).TransformText();
        }
        #endregion

        public bool SendEmail(ObjectId tenantId, string[] to, string subject, string body, string[] cc = null, string[] bcc = null, string reply = null)
        {
            try
            {
                MailerConfiguration mailConfig = Utilities.Instance.GetMailerConfiguration();

                MailMessage mailMessage = new MailMessage();
                SmtpClient smtpServer = new SmtpClient(mailConfig.ClientURL, mailConfig.ClientPort);
                mailMessage.From = new MailAddress(mailConfig.MailAddress);

                if (reply != null)
                {
                    mailMessage.ReplyToList.Add(reply);
                }
                if (cc != null)
                {
                    mailMessage.CC.Add(string.Join(",", cc));
                }
                if (bcc != null)
                {
                    mailMessage.Bcc.Add(string.Join(",", bcc));
                }
                mailMessage.To.Add(to != null && to.Length > 0 ? string.Join(",", to) : mailConfig.MailAddress);
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = Encoding.GetEncoding("utf-8");
                mailMessage.Body = body;

                var tenant = _tenant ?? DynamicHelper.GetTenant(_dbPool, _cache, tenantId);
                if (tenant != null)
                {
                    // If Body contains "cid:Logo_Email", add Theme_Logo_Email to attachments
                    if (body.Contains("cid:Logo_Email"))
                    {
                        var logoAttachment = new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath("~" + tenant.Theme_Logo_Email))
                        {
                            ContentId = "Logo_Email"
                        };
                        mailMessage.Attachments.Add(logoAttachment);
                    }
                    // If Body contains "cid:Bullet", add Theme_Bullet_Image to attachments
                    if (body.Contains("cid:Bullet"))
                    {
                        var bulletAttachment = new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath("~" + tenant.Theme_Bullet_Image))
                        {
                            ContentId = "Bullet"
                        };
                        mailMessage.Attachments.Add(bulletAttachment);
                    }
                }

                smtpServer.EnableSsl = true;
                smtpServer.UseDefaultCredentials = false;
                smtpServer.Credentials = new System.Net.NetworkCredential(mailConfig.Username, mailConfig.Password);

                smtpServer.Send(mailMessage);
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError(ex, string.Join(",", to));
                }
            }

            return true;
        }
    }
}
