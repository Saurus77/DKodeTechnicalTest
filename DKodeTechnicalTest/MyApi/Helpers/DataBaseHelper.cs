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

        // Executes usp asynchronously using dapper
        // Params:
        // - uspName: name of procedure
        // - parameters: object containing params for usp
        public async Task ExecuteUserStoredProcedureAsync(string uspName, object parameters)
        {
            // New db connection
            using var connection = CreateConnection();
            // Exec usp
            await connection.ExecuteAsync(uspName, parameters, commandType: CommandType.StoredProcedure);
        }

        // Executes sql query and returns first value or default value
        // Params:
        // - sql: query to exectue
        // - parameters: obj. containing params for query
        // Generic type T for auto result type handling
        public async Task<T> QueryFirstResultOrDefaultAsync<T>(string sql, object parameters)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
        }
    }
}
