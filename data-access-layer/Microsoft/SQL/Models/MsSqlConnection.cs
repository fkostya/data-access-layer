using data_access_layer.Interface;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace data_access_layer.Microsoft.SQL.Models
{
    public class MsSqlConnection : IConnection<SqlConnectionStringBuilder>
    {
        private readonly SqlConnectionStringBuilder? builder;
        private const int default_timeout_in_seconds = 180;
        private readonly bool isValidConnectionString = true;
        private readonly IDbFactory? factory;
        private DbConnection? instance;

        #region ctor
        public MsSqlConnection(string _connectionString, IDbFactory factory)
        {
            try
            {
                builder = new SqlConnectionStringBuilder(_connectionString);
                this.factory = factory;
            }
            catch {
                isValidConnectionString = false;
            }
        }

        public MsSqlConnection(string dbServer, string dbDatabase, string dbUserName, string dbPassword, IDbFactory factory)
        {
            try
            {
                builder = new SqlConnectionStringBuilder
                {
                    DataSource = dbServer,
                    InitialCatalog = dbDatabase,
                    UserID = dbUserName,
                    Password = dbPassword,
                    ConnectTimeout = default_timeout_in_seconds
                };
                this.factory = factory;
            }
            catch
            {
                isValidConnectionString = false;
            }
        }
        #endregion

        public virtual Task OpenAsync(CancellationToken cancellationToken = default)
        {
            if (isValidConnectionString && builder != null && factory != null)
            {
                try
                {
                    instance =  factory.GetMsSqlDbConnection(builder);
                    if (instance != null && instance.State != System.Data.ConnectionState.Open)
                    {
                        return instance.OpenAsync(cancellationToken);
                    }
                }
                catch { }
            
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

        public SqlConnectionStringBuilder GetConnection()
        {
            return builder ?? [];
        }


        public virtual MsSqlCommandWrapper CreateCommand()
        {
            if (isValidConnectionString && builder != null && factory != null)
            {
                try
                {
                    instance = factory.GetMsSqlDbConnection(builder);
                    if (instance != null)
                    {
                        return new MsSqlCommandWrapper(instance.CreateCommand());
                    }
                }
                catch { }
            }
            return new MsSqlCommandWrapper();
        }

        public virtual Task CloseAsync()
        {
            return instance?.CloseAsync() ?? Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return instance?.DisposeAsync() ?? ValueTask.CompletedTask;
        }

        public bool IsValidConnection()
        {
            if(builder == null || !isValidConnectionString) return false;

            return !(string.IsNullOrEmpty(builder.InitialCatalog) ||
                    string.IsNullOrEmpty(builder.DataSource) ||
                    string.IsNullOrEmpty(builder.UserID) ||
                    string.IsNullOrEmpty(builder.Password));
        }
    }
}