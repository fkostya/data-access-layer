using Microsoft.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace data_access_layer.MsSql
{
    [ExcludeFromCodeCoverage]
    public class SqlConnectionWrapper(string connectionString = "") : IDisposable
    {
        private readonly SqlConnection connection = new(connectionString);

        public static SqlConnectionWrapper Default(string connectionString)
        {
            return new SqlConnectionWrapper(connectionString);
        }

        public virtual async Task OpenAsync(CancellationToken cancellationToken = default)
        {
            await connection.OpenAsync(cancellationToken);
        }

        public virtual SqlCommand CreateCommand()
        {
            return connection.CreateCommand();
        }

        public void Dispose() => connection.Dispose();

        public virtual Task CloseAsync()
        {
            return connection.CloseAsync();
        }
    }
}