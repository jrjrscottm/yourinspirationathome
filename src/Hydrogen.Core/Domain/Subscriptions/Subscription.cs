using Hydrogen.Core.Domain.Consultants;

using System.Collections.Generic;

namespace Hydrogen.Core.Domain.Subscriptions
{
    public class Subscription
    {
        public string SubscriptionId { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }

        public List<ConsultantSubscription> Consultants { get; set; }
        public List<SubscriptionPrice> Prices { get; set; }

    }

    public class SubscriptionPrice
    {
        public string SubscriptionId { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
        public Subscription Subscription { get; set; }

    }


    public class ConsultantSubscription
    {
        public string UserId { get; set; }
        public string SubscriptionId { get; set; }

        public Subscription Subscription { get; set; }
        public Consultant Consultant { get; set; }
    }
}