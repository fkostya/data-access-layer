using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;

namespace data_access_layer.Microsoft.SQL
{
    public class MsSqlDataReaderWrapper(DbDataReader dbReader) : IAsyncDisposable
    {
        private readonly DbDataReader reader = dbReader;

        #region ctor
        public MsSqlDataReaderWrapper() : this(new DbDataReaderEmpty())
        {

        }
        #endregion

        public virtual bool HasRows => reader.HasRows;
        public virtual object this[string key] => reader[key];

        public ValueTask DisposeAsync()
        {
            return reader.DisposeAsync();
        }

        public virtual DataTable? GetSchemaTable()
        {
            return reader.GetSchemaTable();
        }

        public virtual Task<bool> ReadAsync(CancellationToken cancellationToken = default)
        {
            return reader.ReadAsync(cancellationToken);
        }

        public virtual object? GetValueOrDefault(string key, object? @default = default)
        {
            try
            {
                return reader.GetValue(key) ?? @default;
            }
            catch { }

            return @default;
        }

        public virtual Task<ReadOnlyCollection<DbColumn>> GetColumnSchemaAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return reader.GetColumnSchemaAsync(cancellationToken);
            }
            catch { }

            return Task.FromResult(new ReadOnlyCollection<DbColumn>(new List<DbColumn>()));
        }

        public virtual Task<bool> NextResultAsync(CancellationToken cancellationToken = default)
        {
            return reader.NextResultAsync(cancellationToken);
        }

        public virtual void Close()
        {
            reader.Close();
        }
    }
}