using data_access_layer.MsSql;
using log4net;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace data_access_layer
{
    public class MsSqlDataAccessLayer(string connectionString, string userid, string userpassword, int connection_timeout = 1000)
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MsSqlDataAccessLayer));
        private readonly int _connection_timeout = connection_timeout;
        private readonly string _connectionString = connectionString;
        private readonly string _userid = userid;
        private readonly string _userpassword = userpassword;
        private readonly Func<string, SqlConnectionWrapper> _factory = (string connectionString) => SqlConnectionWrapper.Default(connectionString);

        public bool ValidConnection => !string.IsNullOrEmpty(_connectionString);

        #region ctor
        public MsSqlDataAccessLayer(Func<string, SqlConnectionWrapper> factory, Dictionary<string, string> connection)
            : this(connection)
        {
            _factory = factory;
        }

        public MsSqlDataAccessLayer(string connectionString, int connection_timeout = 1000)
            : this(connectionString, "", "", connection_timeout)
        {
        }

        public MsSqlDataAccessLayer(Dictionary<string, string> msSqlAccess, int connection_timeout = 1000)
            : this("", msSqlAccess.GetValueOrDefault("userid") ?? "", msSqlAccess.GetValueOrDefault("userpassword") ?? "", connection_timeout)
        {
            msSqlAccess ??= [];
            //must be specified fields to establish connection to ms sql server
            if (msSqlAccess.TryGetValue("server", out string? server) && msSqlAccess.TryGetValue("database", out string? database))
            {
                _connectionString = $@"Server={server};
                                                Database={database}
                                                    {(msSqlAccess.TryGetValue("port", out string? port) ? $",{port}" : "")};
                                                {(!string.IsNullOrEmpty(_userid) ? $"User Id ={_userid}" : "")};
                                                {(!string.IsNullOrEmpty(_userpassword) ? $"Password ={_userpassword}" : "")};";
            }
        }
        #endregion

        public virtual SqlConnection GetConnection(string connectionString) {
            return new SqlConnection(connectionString);
        }
        
        public async Task<IEnumerable<MsSqlDataSet>> SelectDataAsDataSet(string sql_query_text, CancellationToken cancellationToken = default) {
            if (string.IsNullOrEmpty(sql_query_text) || !ValidConnection)
                return Array.Empty<MsSqlDataSet>();

            Stopwatch sw = new();
            sw.Start();

            try
            {
                var builder = new SqlConnectionStringBuilder(_connectionString)
                {
                    ConnectTimeout = _connection_timeout,
                };

                log.DebugFormat("ConnectionString {ConnectionString} query {Query}", builder, sql_query_text);

                using var connection = _factory.Invoke(builder.ConnectionString);
                await connection.OpenAsync(cancellationToken);
                using var command = connection.CreateCommand();
                command.CommandText = sql_query_text;

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (!reader.HasRows) return Enumerable.Empty<MsSqlDataSet>();

                var list = new List<MsSqlDataSet>();

                do
                {
                    var columns = await reader.GetColumnSchemaAsync(cancellationToken);

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

                await connection.CloseAsync();
                return list;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
            finally
            {
                sw.Stop();
                log.DebugFormat("{SqlTotalExecutionTime} total query run time for {ConnectionString}", sw.Elapsed, _connectionString);
            }

            return Array.Empty<MsSqlDataSet>();
        }
    }
}