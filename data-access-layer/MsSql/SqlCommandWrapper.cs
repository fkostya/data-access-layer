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

        public virtual string CommandText
        {
            set { _command.CommandText = value;}
            get { return _command.CommandText; }
        }

        public virtual async Task<SqlDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default)
        {
            return await _command.ExecuteReaderAsync(cancellationToken);
        }

        public void Dispose() => _command.Dispose();
    }
}