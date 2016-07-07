using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace Hydrogen.Infrastructure.Authorization
{
    public class SubscriptionRequirement : IAuthorizationRequirement {
        public string SubscriptionId { get; set; }

        public SubscriptionRequirement(string subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }

    public class SubscriptionAuthorizationHandler : AuthorizationHandler<SubscriptionRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SubscriptionRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == requirement.SubscriptionId))
                context.Succeed(requirement);

            return Task.FromResult(0);
        }
    }
}
