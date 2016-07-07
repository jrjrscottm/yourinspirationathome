using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Hydrogen.Services.Subscriptions;

namespace Hydrogen.Models
{
    public class ApplicationClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser, IdentityRole>
    {
        readonly ISubscriptionService _subscriptionService;

        public ApplicationClaimsPrincipalFactory(
            UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> optionsAccessor, ISubscriptionService subscriptionService) 
            : base(userManager, roleManager, optionsAccessor)
        {
            this._subscriptionService = subscriptionService;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
        {
            var principal = await base.CreateAsync(user);

            var claimsIdentity = (ClaimsIdentity) principal.Identity;

            claimsIdentity.RemoveClaim(
                    claimsIdentity.FindFirst(x => x.Type == ClaimTypes.Name)
                );

            //TODO: Assign appropriate claims on the identity user
            //claimsIdentity.AddClaims(new[] {
            //    new Claim(ClaimTypes.GivenName, user.FirstName),
            //    new Claim(ClaimTypes.Surname, user.LastName),
            //    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            //    new Claim("ConsultantId", user.ConsultantId)
            //});

            var subscriptions = _subscriptionService.GetActiveSubscriptionsByUserId(user.Id);

            foreach(var subscription in subscriptions)
            {
                claimsIdentity.AddClaim(new Claim(subscription.SubscriptionId, "true"));
            }

            return principal;
        }
    }
}
