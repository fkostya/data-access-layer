namespace data_access_layer.Model
{
    public abstract class DbConnectionString
    {
        public string SessionId { get; init; }
        public string Name { get; init; }
        public string Server { get; init; }
        public int? Port { get; init; }
        public string Database { get; init; }
        protected string? UserId { get; init; }
        protected string? Password { get; init; }
        

        public abstract string ConnectionString { get; }

        protected virtual bool IsValid
        {
            get
            {
                return (
                    !string.IsNullOrEmpty(Server) && !string.IsNullOrEmpty(Database) &&
                    (
                        (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(Password)) ||
                        (string.IsNullOrEmpty(UserId) && string.IsNullOrEmpty(Password))
                    )
                );
            }
        }

        protected virtual string DbServerInstanceAndPort
        {
            get
            {
                return $"Data Source={Server}{((Port == null) ? "" : $";{Port}")}";
            }
        }

        protected virtual string Credentials
        {
            get
            {
                if(string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(UserId))
                {
                    return "Integrated Security=True";
                }
                return @$"Integrated Security=False;User ID={UserId};Password={Password}";
            }
        }

        public DbConnectionString(string name, string server, string database, string uid, string pwd, string sid)
        {
            Name = name;
            Server = server;
            Database = database;
            UserId = uid;
            Password = pwd;
            SessionId = sid;
        }
    }
}