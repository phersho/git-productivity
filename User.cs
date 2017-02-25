using System;
using System.Security.Principal;

namespace api.Core
{
    public class User:IIdentity
    {
        public enum IarUserTypes
        {
            Administrator = 0,
            MasterAdmin,
            Dispatcher,
            User,
            DispatcherUser,
            Apparatus
        }

        public string Name { get; private set; }

        public int MemberId { get; private set; }
        public int SubscriberId { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public string AuthenticationType { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public IarUserTypes UserType { get; private set; }

        public User(string name, int memberId, int subscriberId, DateTime createdOn, IarUserTypes userType, string authenticationType, bool isAuthenticated)
        {
            Name = name;
            MemberId = memberId;
            SubscriberId = subscriberId;
            CreatedOn = createdOn;
            AuthenticationType = authenticationType;
            IsAuthenticated = isAuthenticated;

            UserType = userType;
        }
    }
}