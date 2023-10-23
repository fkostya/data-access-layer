using data_access_layer.Microsoft.SQL.Models;

namespace data_access_layer.Tests.Microsoft.SQL.Model
{
    public class MsSqlConnectionTests
    {
        [Fact]
        public void NewInstanceStringConnectionAsEmpty_NotNull()
        {
            MsSqlConnection connection = new("");
            Assert.NotNull(connection);
        }

        [Fact]
        public void NewInstanceStringConnectionAsNull_NotNull()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            MsSqlConnection connection = new(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.NotNull(connection);
        }

        [Fact]
        public void NewInstanceStringConnectionAsParams_NotNull()
        {
            MsSqlConnection connection = new("server", "database", "userid", "password");
            Assert.NotNull(connection);
        }

        [Fact]
        public void GetConnection_ConnectionBuilderParams()
        {
            MsSqlConnection connection = new("test-server", "test-database", "test-userid", "test-password");
            var connectionBuilder = connection.GetConnection();

            Assert.NotNull(connectionBuilder);
            Assert.Equal("test-server", connectionBuilder.DataSource);
            Assert.Equal("test-database", connectionBuilder.InitialCatalog);
            Assert.Equal("test-userid", connectionBuilder.UserID);
            Assert.Equal("test-password", connectionBuilder.Password);
            Assert.Equal(180, connectionBuilder.ConnectTimeout);
        }

        [Fact]
        public void GetConnectionFromConnectionString_ConnectionBuilderParams()
        {
            MsSqlConnection connection = new("test-server", "test-database", "test-userid", "test-password");
            var connectionBuilder = connection.GetConnection();

            Assert.NotNull(connectionBuilder);
            Assert.Equal("Data Source=test-server;Initial Catalog=test-database;User ID=test-userid;Password=test-password;Connect Timeout=180", connectionBuilder.ConnectionString);
        }

        [Theory]
        [InlineData("", "d", "u", "p", false)]
        [InlineData("s", "", "u", "p", false)]
        [InlineData("s", "d", "", "p", false)]
        [InlineData("s", "d", "u", "", false)]
        [InlineData("s", "d", "u", "p", true)]
        public void IsValidConnection_CheckValidConnections(string server, string db, string user, string pwd, bool result)
        {
            MsSqlConnection connection = new(server, db, user, pwd);
            var valid = connection.IsValidConnection();

            Assert.Equal(result, valid);
        }
    }
}