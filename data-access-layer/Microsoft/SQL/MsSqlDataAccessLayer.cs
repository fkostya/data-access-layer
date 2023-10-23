using data_access_layer.Interface;
using data_access_layer.Microsoft.SQL.Models;
using Microsoft.Data.SqlClient;
using Serilog;
using System.Diagnostics;

namespace data_access_layer.Microsoft.SQL
{
    public class MsSqlDataAccessLayer(MsSqlConnection connection, Func<MsSqlConnection, IDbConnectionWrapper<SqlConnectionStringBuilder>> factory)
    {
        private readonly IDbConnectionWrapper<SqlConnectionStringBuilder> _factory = factory.Invoke(connection);

        #region ctor
        public MsSqlDataAccessLayer(MsSqlConnection connection)
            : this(connection, new Func<MsSqlConnection, SqlConnectionWrapper>((c) => SqlConnectionWrapper.Default(c)))
        {
        }

        //public MsSqlDataAccessLayer(MsSqlConnection connection)
        //{
        //    _connection = connection;
        //    ////must be specified fields to establish connection to ms sql server
        //    //if (msSqlAccess.TryGetValue("server", out string? server) && msSqlAccess.TryGetValue("database", out string? database))
        //    //{
        //    //    _connectionString = $@"Server={server};
        //    //                                    Database={database}
        //    //                                        {(msSqlAccess.TryGetValue("port", out string? port) ? $",{port}" : "")};
        //    //                                    {(!string.IsNullOrEmpty(_userid) ? $"User Id ={_userid}" : "")};
        //    //                                    {(!string.IsNullOrEmpty(_userpassword) ? $"Password ={_userpassword}" : "")};";
        //    //}
        //}
        #endregion

        public async Task<IEnumerable<MsSqlDataSet>> SelectDataAsDataSetAsync(string sql_query_text, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(sql_query_text) || _factory.Connection == null || !await _factory.Connection.IsValidAsync())
                return Array.Empty<MsSqlDataSet>();

            Stopwatch sw = new();
            sw.Start();

            try
            {
                Log.Debug("ConnectionString {@ConnectionString} query {@Query}", _factory.Connection.GetConnection(), sql_query_text);

                await _factory.OpenAsync(cancellationToken);
                await using var command = _factory.CreateCommand();
                if (command == null) return Enumerable.Empty<MsSqlDataSet>();

                command.CommandText = sql_query_text;

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (reader == null || !reader.HasRows) return Enumerable.Empty<MsSqlDataSet>();

                var list = new List<MsSqlDataSet>();

                try
                {
                    do
                    {
                        var columns = await reader.GetColumnSchemaAsync(cancellationToken);
                        if (columns.Count == 0) continue;

                        var dataset = new MsSqlDataSet();
                        foreach (var column in columns)
                        {
                            dataset.AddColumn(column);
                        }

                        while (await reader.ReadAsync(cancellationToken))
                        {
                            //read single row
                            var row = new Dictionary<string, object>();
                            foreach (var column in dataset.Columns)
                            {
                                row[column.Key] = reader[column.Key];
                            }
                            dataset.Add(row);
                        }
                        list.Add(dataset);
                    } while (await reader.NextResultAsync(cancellationToken));
                }
                catch
                {
                    throw;
                }
                finally
                {
                    await _factory.CloseAsync();
                }

                return list;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            finally
            {
                sw.Stop();
                Log.Debug("{@SqlTotalExecutionTime} total query run time for {@ConnectionString}", sw.Elapsed, (_factory.Connection as MsSqlConnection)?.GetConnection().ConnectionString);
            }

            return Array.Empty<MsSqlDataSet>();
        }
    }
}