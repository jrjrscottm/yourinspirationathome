using Hydrogen.Core.Domain.Subscriptions;
using Hydrogen.Domain.Payments;
using System.Collections.Generic;


namespace Hydrogen.Core.Domain.Consultants
{
    public class Consultant
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }

        public string UserId { get; set; }
        public string ConsultantId { get; set; }
        public string ExternalLoginId { get; set;}
        public UserRef User { get; set; }

        public ICollection<PaymentMethod> PaymentMethods { get; set; }
        public ICollection<ConsultantSubscription> Subscriptions { get; set; }
    }

    public class UserRef
    {
        public UserRef(string userId, string email)
        {
            UserId = userId;
            EmailAddress = email;
        }

        public string UserId { get; set; }
        public string EmailAddress { get; set; }
    }
}
