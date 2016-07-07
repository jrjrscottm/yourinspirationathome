using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;

namespace Hydrogen.Infrastructure.Authorization
{
    public class HydrogenClaimsTransformer : IClaimsTransformer
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsTransformationContext context)
        {
            if (context.Principal.Identity.IsAuthenticated)
            {
                context.Principal.Identity.AddClaim("test", "example");
            }

            return Task.FromResult(context.Principal);
        }
    }

    public static class ClaimsExtensions
    {
        public static void AddClaim(this IIdentity identity, string claim, string value)
        {
            (identity as ClaimsIdentity).AddClaim(new Claim(claim, value));
        }
    }
}
