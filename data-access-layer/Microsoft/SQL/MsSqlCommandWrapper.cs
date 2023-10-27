using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace data_access_layer.Microsoft.SQL
{
    [ExcludeFromCodeCoverage]
    public class MsSqlCommandWrapper(DbCommand command) : IAsyncDisposable
    {
        private readonly MsSqlCommandWrapper _command = new(command);

        public MsSqlCommandWrapper()
            : this(new SqlCommand())
        {
        }

        public virtual string CommandText
        {
            set { _command.CommandText = value; }
            get { return _command.CommandText; }
        }

        public virtual async Task<MsSqlDataReaderWrapper> ExecuteReaderAsync(CancellationToken cancellationToken = default)
        {
            return await _command.ExecuteReaderAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _command.DisposeAsync();
        }
    }
}