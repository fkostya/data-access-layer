using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace data_access_layer.Microsoft.SQL.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class MsSqlDataReaderWrapper : IAsyncDisposable
    {
        private readonly DbDataReader _reader;
        private bool empty = false;

        #region ctor
        public MsSqlDataReaderWrapper(DbDataReader reader)
        {
            _reader = reader;
        }

        public MsSqlDataReaderWrapper()
            : this(null)
        {
            empty = true;
        }
        #endregion

        public virtual bool HasRows => _reader.HasRows;
        public virtual object this[string key] => _reader[key];

        public ValueTask DisposeAsync()
        {
            return _reader.DisposeAsync();
        }

        public DataTable? GetSchemaTable()
        {
            return _reader.GetSchemaTable();
        }

        public Task<bool> ReadAsync(CancellationToken cancellationToken = default)
        {
            return _reader.ReadAsync(cancellationToken);
        }

        public object? GetValueOrDefault(string key, object? @default = default)
        {
            try
            {
                //return reader.GetOrdinal(key) >= 0 ? reader.GetValue(key) : @default;
                return _reader.GetValue(key);
            }
            catch
            {
            }

            return @default;
        }

        public async Task<ReadOnlyCollection<DbColumn>> GetColumnSchemaAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _reader.GetColumnSchemaAsync();
            }
            catch
            {
            }

            return new ReadOnlyCollection<DbColumn>(new List<DbColumn>());
        }

        public Task<bool> NextResultAsync(CancellationToken cancellationToken = default)
        {
            return _reader.NextResultAsync(cancellationToken);
        }

        public void Close()
        {
            _reader.Close();
        }
    }
}