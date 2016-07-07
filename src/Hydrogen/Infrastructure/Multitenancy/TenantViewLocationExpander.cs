using System.Collections.Generic;
using System.Linq;
using Hydrogen.Core.Domain.Multitenancy;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Http;

namespace Hydrogen.Infrastructure.Multitenancy
{
    public class TenantViewLocationExpander : IViewLocationExpander
    {
        private const string ThemeKey = "theme", TenantKey = "tenant";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values[ThemeKey]
                = context.ActionContext.HttpContext.GetTenant<ApplicationTenant>()?.Theme;

            context.Values[TenantKey]
                = context.ActionContext.HttpContext.GetTenant<ApplicationTenant>()?.Name.Replace(" ", "-");
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            string theme;
            string tenant;
            var expandedViewLocations = new List<string>();

            var hastheme = context.Values.TryGetValue(ThemeKey, out theme);
            var hastenant = context.Values.TryGetValue(TenantKey, out tenant);

            if (hastenant)
            {
                expandedViewLocations.Add($"/Tenants/{tenant}/{{1}}/{{0}}.cshtml");
                expandedViewLocations.Add($"/Tenants/{tenant}/Shared/{{0}}.cshtml");
            }

            if (hastheme && hastenant)
            {
                expandedViewLocations.Add($"/Themes/{theme}/{tenant}/{{1}}/{{0}}.cshtml");
                expandedViewLocations.Add($"/Themes/{theme}/{tenant}/Shared/{{0}}.cshtml");
                expandedViewLocations.Add($"/Themes/{theme}/{{1}}/{{0}}.cshtml");
                expandedViewLocations.Add($"/Themes/{theme}/Shared/{{0}}.cshtml");
            }
            else if (hastheme)
            {
                expandedViewLocations.Add($"/Themes/{theme}/{{1}}/{{0}}.cshtml");
                expandedViewLocations.Add($"/Themes/{theme}/Shared/{{0}}.cshtml");
            }


            return expandedViewLocations.Concat(viewLocations);
        }
    }
}
