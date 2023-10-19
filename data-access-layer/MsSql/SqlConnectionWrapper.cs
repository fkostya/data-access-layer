using data_access_layer.Interface;
using Microsoft.Data.SqlClient;

namespace data_access_layer.MsSql
{
    public class SqlConnectionWrapper(MsSqlConnection connection) : IDbConnectionWrapper<SqlConnectionStringBuilder>, IDisposable
    {
        private readonly MsSqlConnection _connection = connection;
        private readonly SqlConnection instance = new(connection?.GetConnection()?.ConnectionString);

        public IConnection<SqlConnectionStringBuilder> Connection => _connection;

        public static SqlConnectionWrapper Default(MsSqlConnection connection)
        {
            return new SqlConnectionWrapper(connection);
        }

        public virtual async Task OpenAsync(CancellationToken cancellationToken = default)
        {
            await instance.OpenAsync(cancellationToken);
        }

        public virtual SqlCommandWrapper CreateCommand()
        {
            return new SqlCommandWrapper(instance.CreateCommand());
        }

        public virtual Task CloseAsync()
        {
            return instance.CloseAsync();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            instance.Dispose();
        }
    }
}