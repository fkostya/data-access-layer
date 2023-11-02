using Microsoft.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace data_access_layer.Microsoft.SQL.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class MsSqlCommandWrapper(SqlCommand command) : IAsyncDisposable
    {
        private readonly SqlCommand _command = command;

        #region ctor
        public MsSqlCommandWrapper()
            : this(new SqlCommand())
        {
        }
        #endregion

        public virtual string CommandText
        {
            set { _command.CommandText = value; }
            get { return _command.CommandText; }
        }

        public virtual async Task<MsSqlDataReaderWrapper> ExecuteReaderAsync(CancellationToken cancellationToken = default)
        {
            _command.CommandText = CommandText;
            return new MsSqlDataReaderWrapper(await _command.ExecuteReaderAsync(cancellationToken));
        }

        public ValueTask DisposeAsync()
        {
            return _command.DisposeAsync();
        }
    }
}