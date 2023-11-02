using data_access_layer.Microsoft.SQL.Wrappers;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace data_access_layer.Interface
{
    public interface IMsSqlDbFactory
    {
        SqlConnection GetMsSqlDbConnection(SqlConnectionStringBuilder builder);

        //SqlCommand GetMsSqlCommand();

        //MsSqlDataReaderWrapper GetMsSqlDataReader(SqlDataReader reader);
    }
}