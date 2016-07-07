using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Hydrogen.Core.Domain.Multitenancy;

namespace Hydrogen.Infrastructure.Authorization.Data
{
    public class TenantClaimsRepository : ClaimsRepository
    {
        public TenantClaimsRepository(ApplicationTenant tenant)
            :base(tenant.Database.ConnectionString)
        {
            
        }
    }

    public class ClaimsRepository
    {
        private readonly string _connectionString;

        protected ClaimsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual IEnumerable<Claim> GetUserClaims(string userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var claims = connection.Query<UserClaim>(
                    "Claim_GetAllByUserId",
                    new {userId},
                    commandType: CommandType.StoredProcedure);

                return claims.Select(c => new Claim(c.Name, c.Value)).ToList();
            }
        }
    }

    public class UserClaim
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
