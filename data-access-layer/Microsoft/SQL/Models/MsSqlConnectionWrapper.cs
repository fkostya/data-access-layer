using data_access_layer.Factory;
using data_access_layer.Interface;
using data_access_layer.Microsoft.SQL.Wrappers;
using Microsoft.Data.SqlClient;

namespace data_access_layer.Microsoft.SQL.Models
{
    public class MsSqlConnectionWrapper : IConnectionWrapper<SqlConnectionStringBuilder>
    {
        private readonly SqlConnectionStringBuilder? builder;
        private const int default_timeout_in_seconds = 180; //overridable
        protected IMsSqlDbFactory? factory { get; set; } = new MsSqlDbFactory();
        private SqlConnection? instance;

        #region ctor
        public MsSqlConnectionWrapper(string _connectionString)
        {
            try
            {
                builder = new SqlConnectionStringBuilder(_connectionString);
            }
            catch {
            }
        }

        public MsSqlConnectionWrapper(string dbServer, string dbDatabase, string dbUserName, string dbPassword)
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
            }
            catch
            {
            }
        }

        #endregion

        private bool isValidConnectionString()
        {
            try
            {
                return builder?.ConnectionString != null;
            }
            catch
            {
            }
            return false;
        }

        public virtual Task OpenAsync(CancellationToken cancellationToken = default)
        {
            if (isValidConnectionString() && builder != null && factory != null)
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
            if (isValidConnectionString() && builder != null && factory != null)
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

        public virtual bool IsValidConnection()
        {
            if(builder == null || !isValidConnectionString()) return false;

            return !(string.IsNullOrEmpty(builder.InitialCatalog) ||
                    string.IsNullOrEmpty(builder.DataSource) ||
                    string.IsNullOrEmpty(builder.UserID) ||
                    string.IsNullOrEmpty(builder.Password));
        }
    }
}