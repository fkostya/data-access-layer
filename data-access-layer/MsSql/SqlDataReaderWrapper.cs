using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;

namespace data_access_layer.MsSql
{
    public class SqlDataReaderWrapper(DbDataReader? dbReader) : IAsyncDisposable, IDisposable
    {
        private readonly DbDataReader? reader = dbReader;

        #region ctor
        public SqlDataReaderWrapper() : this(null)
        {

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

        public virtual object? GetValueOrDefault(string key, object? @default = default)
        {
            return reader?.GetValue(key) ?? @default;
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