using data_access_layer.Model;

namespace data_access_layer.Tests.Models
{
    class DbConnectionStringFake(string s, string db, string uid, string pwd, int? port) : DbConnectionString("test", s, db, uid, pwd, port, "session")
    {
        public override string ConnectionString => "";
        
        public DbConnectionStringFake(string s, string db, string uid, string pwd)
            : this(s, db, uid, pwd, null)
        {
        }

        public bool isValid() => base.IsValid;

        public string GetconnectionStringWithPort() => base.DbServerInstanceAndPort;
        public string GetCredentials() => base.Credentials;
    }

    public class DbConnectionStringTests
    {
        [Theory]
        [InlineData("", "d", "u", "p", false)]
        [InlineData("s", "", "u", "p", false)]
        [InlineData("s", "d", "", "p", false)]
        [InlineData("s", "d", "u", "", false)]
        [InlineData("s", "d", "u", "p", true)]
        public void IsValid_CheckValidConnectionArgs(string server, string db, string user, string pwd, bool result)
        {
            DbConnectionStringFake connection = new(server, db, user, pwd);
            var valid = connection.isValid();

            Assert.Equal(result, valid);
        }

        [Theory]
        [InlineData("", null, "Data Source=")]
        [InlineData(null, null, "Data Source=")]
        [InlineData("server", null, "Data Source=server")]
        [InlineData("", 1, "Data Source=;1")]
        [InlineData(null, 1, "Data Source=;1")]
        [InlineData("server", 1, "Data Source=server;1")]
        public void DbServerInstanceAndPort_CompareConnectionStrings(string server, int? port, string expected)
        {
            DbConnectionStringFake connection = new(server, "", "", "",  port);

            Assert.Equal(expected, connection.GetconnectionStringWithPort());
        }

        [Theory]
        [InlineData("", "", "Integrated Security=True")]
        [InlineData(null, null, "Integrated Security=True")]
        [InlineData("pwd", null, "Integrated Security=False;User ID=;Password=pwd")]
        [InlineData("pwd", "", "Integrated Security=False;User ID=;Password=pwd")]
        [InlineData(null, "uid", "Integrated Security=False;User ID=uid;Password=")]
        [InlineData("", "uid", "Integrated Security=False;User ID=uid;Password=")]
        [InlineData("pwd", "uid", "Integrated Security=False;User ID=uid;Password=pwd")]
        public void Credentials_CompareCredentials(string pwd, string uid, string expected)
        {
            DbConnectionStringFake connection = new("", "", uid, pwd);

            Assert.Equal(expected, connection.GetCredentials());
        }
    }
}