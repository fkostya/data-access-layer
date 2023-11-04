using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace data_access_layer.Microsoft.SQL.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class MsSqlDataReaderWrapper(DbDataReader reader) : IAsyncDisposable
    {
        private readonly DbDataReader reader = reader;

        #region ctor
        #endregion

        public virtual bool HasRows => reader.HasRows;
        public virtual object this[string key] => reader[key];

        public ValueTask DisposeAsync()
        {
            return reader.DisposeAsync();
        }

        public DataTable? GetSchemaTable()
        {
            return reader.GetSchemaTable();
        }

        public Task<bool> ReadAsync(CancellationToken cancellationToken = default)
        {
            return reader.ReadAsync(cancellationToken);
        }

        public object? GetValueOrDefault(string key, object? @default = default)
        {
            try
            {
                //return reader.GetOrdinal(key) >= 0 ? reader.GetValue(key) : @default;
                return reader.GetValue(key);
            }
            catch (Exception e)
            {
                int g = 0;
            }

            return @default;
        }

        public async Task<ReadOnlyCollection<DbColumn>> GetColumnSchemaAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await reader.GetColumnSchemaAsync();
            }
            catch (Exception e)
            {
                int g = 0;
            }

            return new ReadOnlyCollection<DbColumn>(new List<DbColumn>());
        }

        public Task<bool> NextResultAsync(CancellationToken cancellationToken = default)
        {
            return reader.NextResultAsync(cancellationToken);
        }

        public void Close()
        {
            reader.Close();
        }
    }
}