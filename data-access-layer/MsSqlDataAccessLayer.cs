using log4net;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Text;

namespace data_access_layer
{
    public class MsSqlDataAccessLayer
    {
        private readonly Dictionary<string, string> _msSqlAccess;
        private readonly bool _trusted_connection = true;
        private static readonly ILog log = LogManager.GetLogger(typeof(MsSqlDataAccessLayer));
        private readonly int _connection_timeout = 1000;

        public MsSqlDataAccessLayer(Dictionary<string, string> msSqlAccess, bool trusted_connection = true, int connection_timeout = 1000)
        {
            _msSqlAccess = msSqlAccess ?? new Dictionary<string, string>();
            _trusted_connection = trusted_connection;
            _connection_timeout = connection_timeout;
        }

        public async Task<MsSqlDataSet> SelectDataAsDataSet(StringBuilder sql_query_text, CancellationToken cancellationToken = default) {
            if (string.IsNullOrEmpty(sql_query_text?.ToString()) || !isSqlQueryTextValid(sql_query_text))
                return MsSqlDataSet.Empty;

            try
            {
                //must be specified fields to establish connection to ms sql server
                if (_msSqlAccess.ContainsKey("server")  && _msSqlAccess.ContainsKey("database"))
                {
                    var connectionString = $@"Server={_msSqlAccess["server"]};
                                                Database={_msSqlAccess["database"]}
                                                    {(_msSqlAccess.ContainsKey("port") ? $",{_msSqlAccess["port"]}" : "")};
                                                Trusted_Connection={_trusted_connection.ToString()};
                                                {(_msSqlAccess.ContainsKey("userid") ? $"User Id ={_msSqlAccess["userid"]}" : "")};
                                                {(_msSqlAccess.ContainsKey("password") ? $"Password ={_msSqlAccess["password"]}" : "")};";

                    var builder = new SqlConnectionStringBuilder(connectionString)
                    {
                        ConnectTimeout = _connection_timeout,
                    };

                    log.Info(builder);

                    using (var connection = new SqlConnection(builder.ConnectionString))
                    {
                        await connection.OpenAsync();

                        SqlCommand command = new SqlCommand(sql_query_text.ToString(), connection);

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
                }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return MsSqlDataSet.Empty;
        }

        private bool isSqlQueryTextValid(StringBuilder sql_query)
        {
            return true;
        }
    }
}