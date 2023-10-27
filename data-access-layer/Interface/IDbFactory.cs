using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace data_access_layer.Interface
{
    public interface IDbFactory
    {
        DbConnection GetMsSqlDbConnection(SqlConnectionStringBuilder builder);
    }
}