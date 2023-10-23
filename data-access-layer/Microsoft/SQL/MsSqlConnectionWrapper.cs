using data_access_layer.Interface;
using data_access_layer.Microsoft.SQL.Models;
using Microsoft.Data.SqlClient;

namespace data_access_layer.Microsoft.SQL
{
    public class MsSqlConnectionWrapper(MsSqlConnection connection) : IDbConnectionWrapper<SqlConnectionStringBuilder>, IAsyncDisposable
    {
        private readonly MsSqlConnection _connection = connection;
        private readonly SqlConnection instance = new(connection?.GetConnection()?.ConnectionString);

        public IConnection<SqlConnectionStringBuilder> Connection => _connection;

        public static MsSqlConnectionWrapper Default(MsSqlConnection connection)
        {
            return new MsSqlConnectionWrapper(connection);
        }

        public virtual async Task OpenAsync(CancellationToken cancellationToken = default)
        {
            await instance.OpenAsync(cancellationToken);
        }

        public virtual MsSqlCommandWrapper CreateCommand()
        {
            return new MsSqlCommandWrapper(instance.CreateCommand());
        }

        public virtual Task CloseAsync()
        {
            return instance.CloseAsync();
        }

        public ValueTask DisposeAsync() =>
            instance.DisposeAsync();
    }
}