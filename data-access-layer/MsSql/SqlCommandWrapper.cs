using Microsoft.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace data_access_layer.MsSql
{
    [ExcludeFromCodeCoverage]
    public class SqlCommandWrapper(SqlCommand command) : IDisposable
    {
        private readonly SqlCommand _command = command;

        public SqlCommandWrapper()
            : this(new SqlCommand())
        {
        }

        public string CommandText
        {
            get
            {
                return _command.CommandText;
            }
            set
            {
                _command.CommandText = value;
            }
        }

        public virtual async Task<SqlDataReaderWrapper> ExecuteReaderAsync(CancellationToken cancellationToken = default)
        {
            return new SqlDataReaderWrapper(await _command.ExecuteReaderAsync(cancellationToken));
        }

        public void Dispose() => _command.Dispose();
    }
}