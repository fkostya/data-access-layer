using data_access_layer.Interface;
using Microsoft.Data.SqlClient;

namespace data_access_layer.Microsoft.SQL.Models
{
    public class MsSqlConnection : IConnection<SqlConnectionStringBuilder>
    {
        private readonly SqlConnectionStringBuilder connection;
        private const int default_timeout_in_seconds = 180;

        #region ctor
        public MsSqlConnection(string _connectionString)
        {
            connection = new SqlConnectionStringBuilder(_connectionString);
        }

        public MsSqlConnection(string dbServer, string dbDatabase, string dbUserName, string dbPassword)
        {
            connection = new SqlConnectionStringBuilder
            {
                DataSource = dbServer,
                InitialCatalog = dbDatabase,
                UserID = dbUserName,
                Password = dbPassword,
                ConnectTimeout = default_timeout_in_seconds
            };
        }
        #endregion

        public SqlConnectionStringBuilder GetConnection()
        {
            return connection;
        }

        public Task<bool> IsValidAsync()
        {
            return Task.FromResult(!(string.IsNullOrEmpty(connection.InitialCatalog) ||
                    string.IsNullOrEmpty(connection.DataSource) ||
                    string.IsNullOrEmpty(connection.UserID) ||
                    string.IsNullOrEmpty(connection.Password)));
        }
    }
}