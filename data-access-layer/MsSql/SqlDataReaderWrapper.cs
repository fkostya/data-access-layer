using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace data_access_layer.MsSql
{
    [ExcludeFromCodeCoverage]
    public class SqlDataReaderWrapper : IAsyncDisposable, IDisposable
    {
        private readonly SqlDataReader? reader;

        #region ctor
        public SqlDataReaderWrapper()
        {

        }

        public SqlDataReaderWrapper(SqlDataReader sqlDataReader)
            : this()
        {
            reader = sqlDataReader;
        }
        #endregion

        public virtual bool HasRows => reader?.HasRows ?? false;
        public virtual object this[string key] => reader?[key] ?? new object();

        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return reader?.DisposeAsync() ?? new ValueTask();
        }

        public virtual DataTable GetSchemaTable()
        {
            return reader?.GetSchemaTable() ?? new DataTable();
        }

        public virtual Task<bool> ReadAsync(CancellationToken cancellationToken = default)
        {
            return reader?.ReadAsync(cancellationToken) ?? Task.FromResult<bool>(false);
        }

        public virtual object GetValue(string key)
        {
            return reader?.GetValue(key) ?? new object();
        }

        public virtual Task<ReadOnlyCollection<DbColumn>> GetColumnSchemaAsync(CancellationToken cancellationToken = default)
        {
            return reader?.GetColumnSchemaAsync(cancellationToken) 
                ?? Task.FromResult(new ReadOnlyCollection<DbColumn>(new List<DbColumn>()));
        }

        public virtual Task<bool> NextResultAsync(CancellationToken cancellationToken = default)
        {
            return reader?.NextResultAsync(cancellationToken) ?? Task.FromResult<bool>(false);
        }

        public virtual void Close()
        {
            reader?.Close();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            reader?.Dispose();
        }
    }
}