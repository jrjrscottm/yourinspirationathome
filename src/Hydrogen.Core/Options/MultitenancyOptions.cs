using System.Collections.ObjectModel;
using Hydrogen.Core.Domain.Multitenancy;

namespace Hydrogen.Infrastructure.Multitenancy
{
    public class MultitenancyOptions
    {
        public string DefaultTenant { get; set; }
        public bool MigrateDatabases { get; set; }
        public Collection<ApplicationTenant> Tenants { get; set; }
    }
}
