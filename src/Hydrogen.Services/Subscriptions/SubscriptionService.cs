using Hydrogen.Core.Domain.Subscriptions;
using Hydrogen.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Hydrogen.Services.Subscriptions
{
    public interface ISubscriptionService
    {
        IEnumerable<Subscription> GetActiveSubscriptionsByUserId(string userId);
        IEnumerable<Subscription> GetAvailableSubscriptions();
    }


    public class SubscriptionService : ISubscriptionService
    {
        readonly HydrogenApplicationContext _context;

        public SubscriptionService(HydrogenApplicationContext context)
        {
            this._context = context;
        }
        public IEnumerable<Subscription> GetAvailableSubscriptions()
        {
            return _context.Subscriptions.ToList();
        }

        public IEnumerable<Subscription> GetActiveSubscriptionsByUserId(string userId)
        {
            var subscriptions = _context.UserSubscriptions
                .Where(x => x.UserId == userId)
                .Include(x => x.Subscription);

            return subscriptions.Select(x => x.Subscription).ToList();
       } 
    }
}
