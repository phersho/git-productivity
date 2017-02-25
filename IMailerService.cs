using System;
using Blitz.Core.Model.Dynamic;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Blitz.Core.Infrastructure.Mailer.Services
{
    public interface IMailerService
    {
        string GetForgotPasswordBody(string email, string callbackUrl, string domainUrl);
        string GetRequestAccessBody(string tenantName, string tenantEmail, string firstName, string lastName,
            string email, string domainUrl);
        string GetRegisterBody(string name, string domainUrl, string userDomain);
        string GetNotifyMarketingBody(string firstName, string lastName, string email, string company, string phone, string domainUrl);
        string GetPaidCommissionsSalesPersonBody(string salesPerson, string manager, Tenant tenant);
        string GetPayCommissionsSalesManagerBody(string salesManager, string transactionType, Tenant tenant);
        string GetTrialExpirationBody(string userFullName, int daysRemaining, string domainUrl);
        string GetSignInNewDeviceBody(string userFullName, string callbackurl, string domainUrl);
        string GetNewUserBody(string userFullName, string userId, Tenant tenant);
        string GetSystemMaintenanceBody(DateTime startDate, DateTime endDate, string domainUrl, string dateFormat);
        string GetNewCompensationPlanBody(string salesPerson, string manager, Tenant tenant);
        string GetSalesTransactionCancelledBody(string salesPerson, string name, DateTime date, string transactionType,
            double amount, Tenant tenant, string dateFormat);
        string GetUpdatedCompensationPlanBody(string salesPerson, Tenant tenant);
        string GetIndividualPaidSalesTransactionBody(string salesPerson, string manager, string name, string transactionType,
            double amount, Tenant tenant);
        string SendUserInvitation(string firstName, string lastName, string email, string link, Tenant tenant);

        #region Daily
        string GetTransactionSummaryBody(List<Dictionary<string, object>> transactionsCountByStatus, List<DynamicRecord> statuses,
            string transactionType, string role, Tenant tenant);
        string GetPaidTransactionListBody(List<Dictionary<string, object>> transactions, string transactionType, Tenant tenant);
        string GetForecastedCommissionsSalesManagerBody(List<Dictionary<string, object>> newInvoices, List<Dictionary<string, object>> updatedInvoices, string manager,
            string transactionType, Tenant tenant);
        string GetDueCommissionsSalespersonBody(List<Dictionary<string, object>> paidTransactions, string name, string transactionType, Tenant tenant);
        string GetDueCommissionsSalesManagerBody(List<Dictionary<string, object>> paidTransactions, string name, string transactionType, Tenant tenant);
        #endregion

        #region Weekly
        string GetWeeklyTransactionSummaryBody(List<Dictionary<string, object>> transactionsCountByStatus, List<DynamicRecord> statuses,
            string transactionType, string role, Tenant tenant);
        string GetWeeklyPaidTransactionListBody(List<Dictionary<string, object>> transactions, string transactionType, Tenant tenant);
        string GetAdminSummaryBody(List<Dictionary<string, object>> lastWeekCommissions, List<Dictionary<string, object>> thisWeekCommissions,
            List<Dictionary<string, object>> lastWeekInvoices, List<Dictionary<string, object>> thisWeekInvoices,
            List<Dictionary<string, object>> lastWeekUserSales, List<Dictionary<string, object>> thisWeekUserSales,
            List<DynamicRecord> invoiceStatuses, List<DynamicRecord> incentiveStatuses, List<DynamicRecord> users, 
            string transactionType, string name, Tenant tenant);
        #endregion

        bool SendEmail(ObjectId tenantId, string[] to, string subject, string body, string[] copy = null, string[] carbonCopy = null, string reply = null);
    }
}
