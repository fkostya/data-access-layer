using log4net;
using log4net.Repository.Hierarchy;
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

        #region ctor
        public MsSqlDataAccessLayer(string connectionString, int connection_timeout = 1000)
            :this(connectionString, "", "", connection_timeout)
        {
                
        }
        public MsSqlDataAccessLayer(Dictionary<string, string> msSqlAccess, int connection_timeout = 1000)
            : this("", msSqlAccess.GetValueOrDefault("userid"), msSqlAccess.GetValueOrDefault("userpassword"), connection_timeout)
        {
            msSqlAccess = msSqlAccess ?? new Dictionary<string, string>();
            //must be specified fields to establish connection to ms sql server
            if (msSqlAccess.ContainsKey("server") && msSqlAccess.ContainsKey("database"))
            {
                _connectionString = $@"Server={msSqlAccess["server"]};
                                                Database={msSqlAccess["database"]}
                                                    {(msSqlAccess.ContainsKey("port") ? $",{msSqlAccess["port"]}" : "")};
                                                {(!string.IsNullOrEmpty(_userid) ? $"User Id ={_userid}" : "")};
                                                {(!string.IsNullOrEmpty(_userpassword) ? $"Password ={_userpassword}" : "")};";
            }
        }
        #endregion

        public async Task<IEnumerable<MsSqlDataSet>> SelectDataAsDataSet(string sql_query_text, CancellationToken cancellationToken = default) {
            if (string.IsNullOrEmpty(sql_query_text) || string.IsNullOrEmpty(_connectionString))
                return Array.Empty<MsSqlDataSet>();

            Stopwatch sw = new();
            sw.Start();

            try
            {
                var builder = new SqlConnectionStringBuilder(_connectionString)
                {
                    ConnectTimeout = _connection_timeout,
                };

                log.Info(builder);

                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    SqlCommand command = new SqlCommand(sql_query_text, connection);

                    var reader = await command.ExecuteReaderAsync(cancellationToken);

                    if(!reader.HasRows) return Array.Empty<MsSqlDataSet>();

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
                    } while (await reader.NextResultAsync());

                    await connection.CloseAsync();
                    return list;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
            finally
            {
                sw.Stop();
                log.Info($"{sw.Elapsed} total query run time for {_connectionString}");
            }

            return Array.Empty<MsSqlDataSet>();
        }
    }
}