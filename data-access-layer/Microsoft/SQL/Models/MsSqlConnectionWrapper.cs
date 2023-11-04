﻿using data_access_layer.Microsoft.SQL.Wrappers;
using data_access_layer.Model;
using Microsoft.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace data_access_layer.Microsoft.SQL.Models
{
    [ExcludeFromCodeCoverage]
    public class MsSqlConnectionWrapper
    {
        public MsSqlConnectionString? Connection { get; init; }
        private readonly SqlConnection? instance;

        #region ctor
        public MsSqlConnectionWrapper(MsSqlConnectionString? connectionString)
        {
            Connection = connectionString;
            if(Connection != null)
            {
                instance = new(Connection.ConnectionString);
            }
        }
        #endregion

        public virtual Task OpenAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return instance?.OpenAsync(cancellationToken) ?? Task.CompletedTask;
            }
            catch { }
            
            return Task.CompletedTask;
        }

        public virtual MsSqlCommandWrapper CreateCommand()
        {
            try
            {
                if(instance != null)
                {
                    return new MsSqlCommandWrapper(instance.CreateCommand());
                }
            }
            catch { }
            return new MsSqlCommandWrapper();
        }

        public virtual Task CloseAsync()
        {
            return instance?.CloseAsync() ?? Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return instance?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
    }
}