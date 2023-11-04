using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace data_access_layer.Microsoft.SQL.Wrappers
{
    public class MsSqlCommandWrapper(DbCommand command) : IAsyncDisposable
    {
        private readonly DbCommand _command = command ?? new SqlCommand();

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
            return new MsSqlDataReaderWrapper(await _command.ExecuteReaderAsync(cancellationToken));
        }

        public ValueTask DisposeAsync()
        {
            return _command.DisposeAsync();
        }
    }
}