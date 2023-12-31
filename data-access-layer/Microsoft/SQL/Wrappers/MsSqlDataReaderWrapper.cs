﻿using Serilog;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;

namespace data_access_layer.Microsoft.SQL.Wrappers
{
    public class MsSqlDataReaderWrapper : IAsyncDisposable
    {
        private readonly DbDataReader? _reader;

        #region ctor
        public MsSqlDataReaderWrapper(DbDataReader? reader)
        {
            _reader = reader;
        }

        public MsSqlDataReaderWrapper()
            : this(null)
        {
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

        public virtual Task<bool> ReadAsync(CancellationToken cancellationToken = default)
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
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message);
            }

            return @default;
        }

        public virtual async Task<ReadOnlyCollection<DbColumn>> GetColumnSchemaAsync(CancellationToken cancellationToken = default)
        {
            if (_reader == null) return await Task.FromResult(new ReadOnlyCollection<DbColumn>(new List<DbColumn>()));
            try
            {
                return await _reader.GetColumnSchemaAsync();
            }
            catch(Exception e)
            {
                Log.Error(e, e.Message);
            }

            return new ReadOnlyCollection<DbColumn>(new List<DbColumn>());
        }

        public virtual Task<bool> NextResultAsync(CancellationToken cancellationToken = default)
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