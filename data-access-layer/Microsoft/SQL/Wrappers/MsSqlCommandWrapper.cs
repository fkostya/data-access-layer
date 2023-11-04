﻿using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace data_access_layer.Microsoft.SQL.Wrappers
{
    public class MsSqlCommandWrapper(DbCommand command) : IAsyncDisposable
    {
        private readonly DbCommand _command = command ?? new SqlCommand();
        private string _commandText;

        #region ctor
        public MsSqlCommandWrapper()
            : this(new SqlCommand())
        {
        }
        #endregion

        public virtual string CommandText
        {
            set { _commandText = value; }
            get { return _commandText; }
        }

        public virtual async Task<MsSqlDataReaderWrapper> ExecuteReaderAsync(CancellationToken cancellationToken = default)
        {
            if(_command != null)
            {
                _command.CommandText = _commandText;
                return new MsSqlDataReaderWrapper(await _command.ExecuteReaderAsync(cancellationToken));
            }
            return await Task.FromResult(new MsSqlDataReaderWrapper());
        }

        public ValueTask DisposeAsync()
        {
            return _command.DisposeAsync();
        }
    }
}