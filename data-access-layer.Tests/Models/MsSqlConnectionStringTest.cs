using AutoFixture;
using data_access_layer.Model;
using FluentAssertions;

namespace data_access_layer.Tests.Models
{
    public class MsSqlConnectionStringTest
    {
        private readonly IFixture _fixture;
        private readonly MsSqlConnectionString _sut;

        public MsSqlConnectionStringTest()
        {
            _fixture = new Fixture();
            _sut = new(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());
        }

        [Fact]
        public void NewInstance_NotNull()
        {
            _sut.Should().NotBeNull();
        }

        [Fact]
        public void NewInstanceWithoutConnectionTimeout_IntegratedSecurityAsFalse_CompareConnectionString()
        {
            var sut = new MsSqlConnectionString("u-test", "u-server", "u-database", "u-userid", "u-password", "u-sessionid");

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

            Assert.Equal("Data Source=u-server;Initial Catalog=u-database;Integrated Security=False;User ID=u-userid;Password=u-password;Connect Timeout=180", sut.ConnectionString);
        }

        [Fact]
        public void NewInstanceWithConnectionTimeout_IntegratedSecurityAsFalse_CompareConnectionString()
        {
            var sut = new MsSqlConnectionString("u-test", "u-server", "u-database", "u-userid", "u-password", "u-sessionid", 200);

            Assert.Equal("Data Source=u-server;Initial Catalog=u-database;Integrated Security=False;User ID=u-userid;Password=u-password;Connect Timeout=200", sut.ConnectionString);
        }

        [Fact]
        public void NewInstanceWithoutConnectionTimeout_IntegratedSecurityAsTrue_CompareConnectionString()
        {
            var sut = new MsSqlConnectionString("u-test", "u-server", "u-database", "u-sessionid");

            Assert.Equal("Data Source=u-server;Initial Catalog=u-database;Integrated Security=True;Connect Timeout=180", sut.ConnectionString);
        }

        [Fact]
        public void NewInstanceWithConnectionTimeout_IntegratedSecurityAsTrue_CompareConnectionString()
        {
            var sut = new MsSqlConnectionString("u-test", "u-server", "u-database", "u-sessionid", 200);

            Assert.Equal("Data Source=u-server;Initial Catalog=u-database;Integrated Security=True;Connect Timeout=200", sut.ConnectionString);
        }

        [Fact]
        public void NewInstance_NotValidServer_EmptyString()
        {
            var sut = new MsSqlConnectionString("u-test", "", "u-database", "u-sessionid");

            sut.ConnectionString.Should().BeEmpty();
        }

        [Fact]
        public void NewInstance_NotValidDatbase_EmptyString()
        {
            var sut = new MsSqlConnectionString("u-test", "u-server", "", "u-sessionid");

            sut.ConnectionString.Should().BeEmpty();
        }
    }
}