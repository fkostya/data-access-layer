using data_access_layer.Model;

namespace data_access_layer.Tests.Models
{
    public class MsSqlConnectionStringTest
    {
        [Fact]
        public void NewInstance_NotNull()
        {
            var connectionString = new MsSqlConnectionString("u-test", "u-server", "u-database", "u-userid", "u-password", "u-sessionid");

            Assert.NotNull(connectionString);
        }

        [Fact]
        public void NewInstanceWithoutConnectionTimeout_IntegratedSecurityAsFalse_CompareConnectionString()
        {
            var connectionString = new MsSqlConnectionString("u-test", "u-server", "u-database", "u-userid", "u-password", "u-sessionid");

            //generate valid connection string
            //SqlConnectionStringBuilder builder = new()
            //{
            //    DataSource = "u-server",
            //    InitialCatalog = "u-database",
            //    IntegratedSecurity = false,
            //    UserID = "u-userid",
            //    Password = "u-password", 
            //    ConnectTimeout = 100,
            //};

            Assert.Equal("Data Source=u-server;Initial Catalog=u-database;Integrated Security=False;User ID=u-userid;Password=u-password;Connect Timeout=180", connectionString.ConnectionString);
        }

        [Fact]
        public void NewInstanceWithConnectionTimeout_IntegratedSecurityAsFalse_CompareConnectionString()
        {
            var connectionString = new MsSqlConnectionString("u-test", "u-server", "u-database", "u-userid", "u-password", "u-sessionid", 200);

            Assert.Equal("Data Source=u-server;Initial Catalog=u-database;Integrated Security=False;User ID=u-userid;Password=u-password;Connect Timeout=200", connectionString.ConnectionString);
        }

        [Fact]
        public void NewInstanceWithoutConnectionTimeout_IntegratedSecurityAsTrue_CompareConnectionString()
        {
            var connectionString = new MsSqlConnectionString("u-test", "u-server", "u-database", "u-sessionid");

            Assert.Equal("Data Source=u-server;Initial Catalog=u-database;Integrated Security=True;Connect Timeout=180", connectionString.ConnectionString);
        }

        [Fact]
        public void NewInstanceWithConnectionTimeout_IntegratedSecurityAsTrue_CompareConnectionString()
        {
            var connectionString = new MsSqlConnectionString("u-test", "u-server", "u-database", "u-sessionid", 200);

            Assert.Equal("Data Source=u-server;Initial Catalog=u-database;Integrated Security=True;Connect Timeout=200", connectionString.ConnectionString);
        }

        [Fact]
        public void NewInstance_NotValidServer_EmptyString()
        {
            var connectionString = new MsSqlConnectionString("u-test", "", "u-database", "u-sessionid");

            Assert.Equal("", connectionString.ConnectionString);
        }

        [Fact]
        public void NewInstance_NotValidDatbase_EmptyString()
        {
            var connectionString = new MsSqlConnectionString("u-test", "u-server", "", "u-sessionid");

            Assert.Equal("", connectionString.ConnectionString);
        }
    }
}
