using Microsoft.Data.SqlClient;
using System.Data;
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
        {
            reader = sqlDataReader;
        }
        #endregion

        public ValueTask DisposeAsync()
        {
            if (reader != null)
                return reader.DisposeAsync();

            return new ValueTask();
        }

        public virtual DataTable GetSchemaTable()
        {
            return reader?.GetSchemaTable() ?? new DataTable();
        }

        public virtual async Task<bool> ReadAsync()
        {
            return (reader != null) ? await reader.ReadAsync() : await Task.FromResult(false);
        }

        public virtual bool HasRows
        {
            get
            {
                return reader != null && reader.HasRows;
            }
        }

        public virtual object GetValue(string key)
        {
            return reader != null ? reader.GetValue(key) : new object();
        }

        public virtual void Close()
        {
            reader?.Close();
        }

        public void Dispose() => reader?.Dispose();
    }
}