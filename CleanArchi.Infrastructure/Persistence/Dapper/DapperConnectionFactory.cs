using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CleanArchi.Infrastructure.Persistence.Dapper
{
    public class DapperConnectionFactory
    {
        private readonly string _connectionString;

        public DapperConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")!;
        }

        public IDbConnection Create()
            => new SqlConnection(_connectionString);
    }
}
