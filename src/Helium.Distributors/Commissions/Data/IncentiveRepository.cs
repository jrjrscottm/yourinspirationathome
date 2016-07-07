using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Helium.Distributors.Commissions.Data.Models;

namespace Helium.Distributors.Commissions.Data
{
    public interface IIncentiveRepository
    {
        IEnumerable<IncentiveReadModel> GetAllIncentives();
        IEnumerable<IncentiveReadModel> GetIncentivesByCommissionTierId(int commissionTierId);
        IncentiveMatrixValueReadModel GetIncentiveMatrixValues(int incentiveId);
        IncentiveTieredValueReadModel<int, decimal> GetIncentiveTieredValues(int incentiveId);
    }

    public class IncentiveRepository : IIncentiveRepository
    {
        private readonly IRepositoryConnectionFactory _connectionFactory;

        public IncentiveRepository(IRepositoryConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<IncentiveReadModel> GetAllIncentives()
        {
            using (var connection = _connectionFactory.CreateOpenConnection())
            {
                return connection.Query<IncentiveReadModel>(
                    "Incentive_GetAll",
                    commandType: CommandType.StoredProcedure
                    ).ToList();
            }
        }

        public IEnumerable<IncentiveReadModel> GetIncentivesByCommissionTierId(int commissionTierId)
        {
            using (var connection = _connectionFactory.CreateOpenConnection())
            {
                return connection.Query<IncentiveReadModel>(
                    "Incentive_GetByCommissionTierId",
                    new {Id = commissionTierId},
                    commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public IncentiveMatrixValueReadModel GetIncentiveMatrixValues(int incentiveId)
        {
            using (var connection = _connectionFactory.CreateOpenConnection())
            {
                using (var reader = connection.QueryMultiple(
                    "Incentive_MatrixValue_GetByIncentiveId",
                    new {Id = incentiveId},
                    commandType: CommandType.StoredProcedure))
                {
                    var matrixValues = reader.Read<List<decimal>>().ToList();

                    dynamic incentiveProperties = reader.ReadFirst();

                    return new IncentiveMatrixValueReadModel(matrixValues, matrixValues.Count)
                    {
                        Id = incentiveProperties.Id,
                        Name = incentiveProperties.Name,
                        Description = incentiveProperties.Description
                    };
                }
            }
        }

        public IncentiveTieredValueReadModel<int, decimal> GetIncentiveTieredValues(int incentiveId)
        {
            using (var connection = _connectionFactory.CreateOpenConnection())
            {
                using (var reader = connection.QueryMultiple(
                    "Incentive_TieredValue_GetByIncentiveId",
                    new {Id = incentiveId},
                    commandType: CommandType.StoredProcedure))
                {
                    var tierValues =
                        reader.Read<int, decimal, dynamic>((k, v) => new {Key = k, Value = v})
                            .ToDictionary(k => (int) k.Key, v => (decimal) v.Value);

                    var incentiveProperties = reader.ReadFirst();

                    return new IncentiveTieredValueReadModel<int, decimal>()
                    {
                        Id = incentiveProperties.Id,
                        Name = incentiveProperties.Name,
                        Description = incentiveProperties.Description,
                        Values = tierValues
                    };
                }
            }

        }
    }
}