using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace data_access_layer.Microsoft.SQL.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class MsSqlDataReaderWrapper : IAsyncDisposable
    {
        private readonly DbDataReader? _reader;
        private bool empty = false;

        #region ctor
        public MsSqlDataReaderWrapper(DbDataReader? reader)
        {
            _reader = reader;
        }

        public MsSqlDataReaderWrapper()
            : this(null)
        {
            empty = true;
        }
        #endregion

        public virtual bool HasRows => _reader?.HasRows ?? false;
        public virtual object this[string key] => _reader?[key] ?? new object();

        public ValueTask DisposeAsync()
        {
            return _reader?.DisposeAsync() ?? ValueTask.CompletedTask;
        }

        public DataTable? GetSchemaTable()
        {
            if (_reader == null) return new DataTable();

            return _reader.GetSchemaTable();
        }

        public Task<bool> ReadAsync(CancellationToken cancellationToken = default)
        {
            if(_reader == null) return Task.FromResult(false);

            return _reader.ReadAsync(cancellationToken);
        }

        public object? GetValueOrDefault(string key, object? @default = default)
        {
            try
            {
                return _reader?.GetValue(key) ?? @default;
            }
            catch
            {
            }

            return @default;
        }

        public async Task<ReadOnlyCollection<DbColumn>> GetColumnSchemaAsync(CancellationToken cancellationToken = default)
        {
            if (_reader == null) return await Task.FromResult(new ReadOnlyCollection<DbColumn>(new List<DbColumn>()));
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
            if (_reader == null) return Task.FromResult(false);

            return _reader.NextResultAsync(cancellationToken);
        }

        public void Close()
        {
            _reader?.Close();
        }
    }
}