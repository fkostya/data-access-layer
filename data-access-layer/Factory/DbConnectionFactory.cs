using data_access_layer.Interface;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace data_access_layer.Factory
{
    public class DbConnectionFactory : IDbFactory
    {
        public DbConnection GetMsSqlDbConnection(SqlConnectionStringBuilder builder)
        {
            return new SqlConnection(builder?.ConnectionString);
        }
    }
}