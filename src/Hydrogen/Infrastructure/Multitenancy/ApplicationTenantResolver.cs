using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Hydrogen.Core.Domain.Multitenancy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;

namespace Hydrogen.Infrastructure.Multitenancy
{
    public interface IApplicationTenantResolver : ITenantResolver<ApplicationTenant>
    {
        Collection<ApplicationTenant> Tenants { get; }
    }

    public class ApplicationTenantResolver : IApplicationTenantResolver
    {
        private readonly IHostingEnvironment _environment;
        public Collection<ApplicationTenant> Tenants { get; }

        public ApplicationTenantResolver(IHostingEnvironment environment, IOptions<MultitenancyOptions> options)
        {
            _environment = environment;
            Tenants = options.Value.Tenants;
        }

        public Task<TenantContext<ApplicationTenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<ApplicationTenant> tenantContext = null;

            var tenant = Tenants.FirstOrDefault(t =>
                t.Hostnames.Any(h => h.Equals(context.Request.Host.Value.ToLower())));

            if (tenant != null)
            {
                tenantContext = new TenantContext<ApplicationTenant>(tenant);
            }
            else if(_environment.IsDevelopment() || _environment.IsEnvironment("Azure"))
            {
                tenantContext = new TenantContext<ApplicationTenant>(new ApplicationTenant
                {
                    Name = "Nucleon",
                    Id = "7704768",
                    Theme = "Nucleon"
                });
            }

            return Task.FromResult(tenantContext);
        }
    }
}
