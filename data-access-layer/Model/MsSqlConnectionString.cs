using System.Text;

namespace data_access_layer.Model
{
    public class MsSqlConnectionString(string name, string server, string database, string uid, string pwd, string sid) : DbConnectionString(name, server, database, uid, pwd, sid)
    {
        protected int ConnectionTimeout { get; init; } = 180;

        #region ctor
        public MsSqlConnectionString(string name, string server, string database, string sid)
            : this(name, server, database, "", "", sid)
        {
        }

        public MsSqlConnectionString(string name, string server, string database, string uid, string pwd, string sid, int timeout)
            : this(name, server, database, uid, pwd, sid)
        {
            ConnectionTimeout = timeout;
        }

        public MsSqlConnectionString(string name, string server, string database, string sid, int timeout)
            : this(name, server, database, "", "", sid)
        {
            ConnectionTimeout = timeout;
        }
        #endregion

        public override string ConnectionString
        {
            get
            {
                if (IsValid)
                {
                    StringBuilder sb = new();
                    sb.Append($"{DbServerInstanceAndPort};");
                    if (!string.IsNullOrEmpty(Database)){
                        sb.Append($"Initial Catalog={Database};");
                    }
                    sb.Append($"{Credentials};");
                    sb.Append($"Connect Timeout={ConnectionTimeout}");

                    return sb.ToString();
                }
                return "";
            }
        }
    }
}