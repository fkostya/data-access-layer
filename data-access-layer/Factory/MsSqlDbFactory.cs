using data_access_layer.Interface;
using data_access_layer.Microsoft.SQL.Wrappers;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace data_access_layer.Factory
{
    public class MsSqlDbFactory : IMsSqlDbFactory
    {
        //public SqlCommand GetMsSqlCommand()
        //{
        //    return new SqlCommand();
        //}

        //public MsSqlDataReaderWrapper GetMsSqlDataReader(SqlDataReader reader)
        //{
        //    return new MsSqlDataReaderWrapper(reader);
        //}

        public SqlConnection GetMsSqlDbConnection(SqlConnectionStringBuilder builder)
        {
            return new SqlConnection(builder?.ConnectionString);
        }
    }
}