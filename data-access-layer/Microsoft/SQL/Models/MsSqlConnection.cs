using data_access_layer.Interface;
using Microsoft.Data.SqlClient;

namespace data_access_layer.Microsoft.SQL.Models
{
    public class MsSqlConnection : IConnection<SqlConnectionStringBuilder>
    {
        private readonly SqlConnectionStringBuilder? connection;
        private const int default_timeout_in_seconds = 180;
        private readonly bool isValidConnectionString = true;

        #region ctor
        public MsSqlConnection(string _connectionString)
        {
            try
            {
                connection = new SqlConnectionStringBuilder(_connectionString);
            }
            catch {
                isValidConnectionString = false;
            }
        }

        public MsSqlConnection(string dbServer, string dbDatabase, string dbUserName, string dbPassword)
        {
            try
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
            catch
            {
                isValidConnectionString = false;
            }
        }
        #endregion

        public SqlConnectionStringBuilder GetConnection()
        {
            return connection;
        }

        public bool IsValidConnection()
        {
            if(connection == null || !isValidConnectionString) return false;

            return !(string.IsNullOrEmpty(connection.InitialCatalog) ||
                    string.IsNullOrEmpty(connection.DataSource) ||
                    string.IsNullOrEmpty(connection.UserID) ||
                    string.IsNullOrEmpty(connection.Password));
        }
    }
}