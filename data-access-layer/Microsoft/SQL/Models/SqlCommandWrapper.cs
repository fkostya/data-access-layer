using Microsoft.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace data_access_layer.Microsoft.SQL.Models
{
    [ExcludeFromCodeCoverage]
    public class SqlCommandWrapper(SqlCommand command) : IAsyncDisposable
    {
        private readonly SqlCommandWrapper _command = new(command);

        public SqlCommandWrapper()
            : this(new SqlCommand())
        {
        }

        public virtual string CommandText
        {
            set { _command.CommandText = value; }
            get { return _command.CommandText; }
        }

        public virtual async Task<SqlDataReaderWrapper> ExecuteReaderAsync(CancellationToken cancellationToken = default)
        {
            return await _command.ExecuteReaderAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _command.DisposeAsync();
        }
    }
}