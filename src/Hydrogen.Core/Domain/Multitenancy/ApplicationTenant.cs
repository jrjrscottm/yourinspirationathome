using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Core.Domain.Multitenancy
{
    public class ApplicationTenant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Hostnames { get; set; }
        public DatabaseSettings Database { get; set; }
        public string Theme { get; set; }
        public EmailProperties Email { get; set; }

        public string[] SupportedLoginTypes { get; set; }
    }

    public class DatabaseSettings
    {
        public string ConnectionString  { get; set; }
    }

    public class EmailProperties
    {
        public string FromAddress { get; set; }
    }
}
