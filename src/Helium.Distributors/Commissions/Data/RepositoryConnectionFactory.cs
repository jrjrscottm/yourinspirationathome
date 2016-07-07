using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Helium.Distributors.Commissions.Data
{
    public interface IRepositoryConnectionFactory
    {
        IDbConnection CreateConnection();
        IDbConnection CreateOpenConnection();
    }

    public interface IAsyncRepositoryConnectionFactory
    {
        Task<IDbConnection> CreateOpenConnectionAsync();
    }

    public class RepositoryConnectionFactory
        :IRepositoryConnectionFactory, IAsyncRepositoryConnectionFactory
    {
        private readonly string _connectionString;

        public RepositoryConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration["Data:DefaultConnection:ConnectionString"];
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<IDbConnection> CreateOpenConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }


        public IDbConnection CreateOpenConnection()
        {
            var connection = CreateConnection();
            connection.Open();
            return connection;
        }
    }
}
