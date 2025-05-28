using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace MyApi.Helpers
{
    // encapsulates database logic
    public class DataBaseHelper
    {
        private readonly string _connectionString;

        // DI configuration obj. to grab connection string from appsettings.json
        public DataBaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
