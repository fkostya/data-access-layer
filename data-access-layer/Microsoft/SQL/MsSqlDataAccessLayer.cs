using data_access_layer.Microsoft.SQL.Models;
using Serilog;
using System.Diagnostics;

namespace data_access_layer.Microsoft.SQL
{
    public class MsSqlDataAccessLayer(MsSqlConnectionWrapper connection)
    {
        private readonly MsSqlConnectionWrapper _connection = connection;

        #region ctor
        #endregion

        public async Task<IEnumerable<MsSqlDataSet>> RunSqlQueryAsDataSetAsync(string sql_query_text, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(sql_query_text) || _connection == null)
                return Array.Empty<MsSqlDataSet>();

            Stopwatch sw = new();
            sw.Start();

            try
            {
                Log.Debug("ConnectionString {@ConnectionString} query {@Query}", _connection.Connection, sql_query_text);

                await _connection.OpenAsync(cancellationToken);
                await using var command = _connection.CreateCommand();
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
                    await _connection.CloseAsync();
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
                Log.Debug("{@SqlTotalExecutionTime} total query run time for {@ConnectionString}", sw.Elapsed, _connection.Connection);
            }

            return Array.Empty<MsSqlDataSet>();
        }
    }
}