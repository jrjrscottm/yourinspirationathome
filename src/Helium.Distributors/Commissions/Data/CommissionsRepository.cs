using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Helium.Distributors.Commissions.Data.Models;

namespace Helium.Distributors.Commissions.Data
{
    public interface ICommissionsRepository
    {
        IEnumerable<CommissionTierReadModel> GetTiers();
    }

    public class CommissionsRepository : ICommissionsRepository
    {
        private readonly IRepositoryConnectionFactory _connectionFactory;


        public CommissionsRepository(IRepositoryConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<CommissionTierReadModel> GetTiers()
        {
            using (var connection = _connectionFactory.CreateOpenConnection())
            {
                return connection.Query<CommissionTierReadModel>(
                    "CommissionTier_GetAll",
                    commandType: CommandType.StoredProcedure).ToList();
            }
        }
    }
}
