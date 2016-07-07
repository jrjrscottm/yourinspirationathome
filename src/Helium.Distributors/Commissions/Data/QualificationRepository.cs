using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Helium.Distributors.Commissions.Data.Models;

namespace Helium.Distributors.Commissions.Data
{
    public interface IQualificationRepository
    {
        IEnumerable<VolumeQualificationReadModel> GetVolumeQualifications();
        IEnumerable<VolumeQualificationReadModel> GetVolumeQualificationsByCommissionTierId(int commissionTierId);
    }

    public class QualificationRepository : IQualificationRepository
    {
        private readonly IRepositoryConnectionFactory _connectionFactory;

        public QualificationRepository(IRepositoryConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        public IEnumerable<VolumeQualificationReadModel> GetVolumeQualifications()
        {
            using (var connection = _connectionFactory.CreateOpenConnection())
            {
                return connection.Query<VolumeQualificationReadModel>(
                    "VolumeQualifications_GetAll",
                    commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public IEnumerable<VolumeQualificationReadModel> GetVolumeQualificationsByCommissionTierId(int commissionTierId)
        {
            using (var connection = _connectionFactory.CreateOpenConnection())
            {
                return connection.Query<VolumeQualificationReadModel>(
                    "VolumeQualifications_GetByCommissionTierId",
                    new {Id = commissionTierId},
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
