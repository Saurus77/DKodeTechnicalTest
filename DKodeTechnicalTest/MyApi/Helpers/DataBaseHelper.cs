using System.Data; // Database access interfaces
using System.Data.SqlClient; // SQL server database access
using Dapper; // ORM for database access
using Microsoft.Extensions.Configuration; // Config access for appsettings.json

namespace MyApi.Helpers
{
    // Encapsulates database logic
    public class DataBaseHelper
    {
        //
        private readonly string _connectionString;

        // DI configuration obj. instance to grab connection string from appsettings.json
        public DataBaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Creates and return sql connection instance using connection string
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Executes usp asynchronously using dapper
        /// </summary>
        /// <param name="uspName">name of procedure</param>
        /// <param name="parameters">object containing params for usp</param>
        public async Task ExecuteUserStoredProcedureAsync(string uspName, object parameters)
        {
            // New db connection
            using var connection = CreateConnection();
            // Exec usp
            await connection.ExecuteAsync(uspName, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
