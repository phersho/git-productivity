using models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Core.Utils
{
    public class DNDMemberSettings
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// It lets get messages available or blocked for a specific member.
        /// </summary>
        /// <param name="memberId">Member id</param>        
        /// <param name="messageId">Message id</param>
        /// <param name="_recipientsMessageTypes">The do not disturb setting for each member in the message</param>
        /// <param name="status">Indicates whether available or blocked messages are obtained.</param>
        /// <param name="processtype"></param>
        /// <returns>Messages available or blocked for a specific member.</returns>
        public string GetMemberAddresses(long memberId, long messageId, IEnumerable<MemberMessageSettings> _recipientsMessageTypes, DNDstatus status, string processtype = "all")
        {
            string addresses = string.Empty;
            var memberMessageSettingses = _recipientsMessageTypes as IList<MemberMessageSettings> ?? _recipientsMessageTypes.ToList();

            try
            {
                var memberDNDsettings = memberMessageSettingses.First(c => c.MemberId == memberId);

                if (memberDNDsettings != null)
                    addresses = status == DNDstatus.Enabled ? memberDNDsettings.EnableMessageTypes : memberDNDsettings.QueueMessageTypes;
                _logger.Debug("memberid: {0} messageid: {1} DNDstatus: {2} processtype: {3}", memberId, messageId, status, processtype);
            }
            catch (Exception)
            {
                var t = memberMessageSettingses.First();
                _logger.Debug("EXCEPTION count: {3} memberid: {0} messageid: {1} DNDstatus: {2} processtype: {5} otherMemberID: {4}",
                    memberId, messageId, status, memberMessageSettingses.Count(), t.MemberId, processtype);
            }

            return addresses;
        }

        /// <summary>
        /// It lets get messages available or blocked for a specific member.
        /// </summary>
        /// <param name="memberUserName">Member user name</param>        
        /// <param name="messageId">Message id</param>
        /// <param name="_recipientsMessageTypes">The do not disturb setting for each member in the message</param>
        /// <param name="status">Indicates whether available or blocked messages are obtained.</param>
        /// <param name="processtype"></param>
        /// <returns>Messages available or blocked for a specific member.</returns>
        public string GetMemberAddresses(string memberUserName, long messageId, IEnumerable<MemberMessageSettings> _recipientsMessageTypes, DNDstatus status, string processtype = "all")
        {
            string addresses = string.Empty;
            var memberMessageSettingses = _recipientsMessageTypes as IList<MemberMessageSettings> ?? _recipientsMessageTypes.ToList();

            try
            {
                var memberDNDsettings = memberMessageSettingses.First(c => c.MemberUserName == memberUserName);

                if (memberDNDsettings != null)
                    addresses = status == DNDstatus.Enabled ? memberDNDsettings.EnableMessageTypes : memberDNDsettings.QueueMessageTypes;
                _logger.Debug("memberUserName: {0} messageid: {1} DNDstatus: {2} processtype: {3}", memberUserName, messageId, status, processtype);
            }
            catch (Exception)
            {
                var t = memberMessageSettingses.First();
                _logger.Debug("EXCEPTION count: {3} memberUserName: {0} messageid: {1} DNDstatus: {2} processtype: {5} otherMemberID: {4}",
                    memberUserName, messageId, status, memberMessageSettingses.Count(), t.MemberId, processtype);
            }

            return addresses;
        }

        /// <summary>
        /// A comparison between the types of messages you want to send and the types of messages that the user can receive is performed.
        /// </summary>
        /// <param name="messageTypes">Types of messages you want to send.</param>
        /// <param name="messageTypesSend">Types of messages that the user can receive.</param>
        /// <returns>A comma separated list of messages you wish to send and the user can receive.</returns>
        public IEnumerable<string> GetAvailableMessageTypes(string[] messageTypes, string[] messageTypesSend)
        {
            var l2Lookup = new HashSet<string>(messageTypesSend);
            var listCommon = messageTypes.Where(l2Lookup.Contains).Select(s => s.Replace("secondary", "email")).ToArray().Distinct();

            return listCommon;
        }
    }

    /// <summary>
    /// Enabled indicates that the member can receive messages.
    /// Purging is set for locked messages, this means that will not be sent to the member.
    /// </summary>
    public enum DNDstatus
    {
        Enabled,
        Purged
    }

    /// <summary>
    /// Types of messages allowed in the SP GetMessageTypesByMember
    /// </summary>
    public enum DNDMessageTypes
    {
        OUTBOUNDMESSAGE,
        INCIDENT,
        DISPATCHEROUTBOUNDMESSAGE
    }
}
