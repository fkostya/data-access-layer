using log4net;
using Microsoft.Data.SqlClient;

namespace data_access_layer
{
    public class MsSqlDataAccessLayer(string connectionString, int connection_timeout = 1000)
    {
        private readonly Dictionary<string, string> msSqlAccess;
        private static readonly ILog log = LogManager.GetLogger(typeof(MsSqlDataAccessLayer));
        private readonly int _connection_timeout = connection_timeout;
        private readonly string _connectionString = connectionString;

        public MsSqlDataAccessLayer(Dictionary<string, string> msSqlAccess, int connection_timeout = 1000)
            :this("", connection_timeout)
        {
            msSqlAccess = msSqlAccess ?? new Dictionary<string, string>();
            //must be specified fields to establish connection to ms sql server
            if (msSqlAccess.ContainsKey("server") && msSqlAccess.ContainsKey("database"))
            {
                _connectionString = $@"Server={msSqlAccess["server"]};
                                                Database={msSqlAccess["database"]}
                                                    {(msSqlAccess.ContainsKey("port") ? $",{msSqlAccess["port"]}" : "")};
                                                {(msSqlAccess.ContainsKey("userid") ? $"User Id ={msSqlAccess["userid"]}" : "")};
                                                {(msSqlAccess.ContainsKey("password") ? $"Password ={msSqlAccess["password"]}" : "")};";
            }
        }

        public async Task<MsSqlDataSet> SelectDataAsDataSet(string sql_query_text, CancellationToken cancellationToken = default) {
            if (string.IsNullOrEmpty(sql_query_text) || !isSqlQueryTextValid(sql_query_text))
                return MsSqlDataSet.Empty;

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

                    if(!reader.HasRows) return MsSqlDataSet.Empty;
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

                    await connection.CloseAsync();

                    return dataset;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return MsSqlDataSet.Empty;
        }

        private bool isSqlQueryTextValid(string sql_query)
        {
            return true;
        }
    }
}