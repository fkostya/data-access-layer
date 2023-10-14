using data_access_layer;
using data_access_layer.MsSql;
using Microsoft.Data.SqlClient;
using Moq;
using System.Data.Common;

namespace data_access_layer_tests
{
    public class MsSqlDataAccessLayerTests
    {
        [Fact]
        public void MsSqlDataAccessLayer_NewInstanceWithNull_NotNull()
        {
            Assert.NotNull(() => new MsSqlDataAccessLayer(string.Empty));
        }

        [Fact]
        public void MsSqlDataAccessLayer_NewInstanceWithConnection_NotNull()
        {
            Assert.NotNull(() => new MsSqlDataAccessLayer("connectionString"));
        }

        [Fact]
        public void MsSqlDataAccessLayer_NewInstanceWithConnectioAndUserNamePassword_NotNull()
        {
            Assert.NotNull(() => 
                new MsSqlDataAccessLayer(new Dictionary<string, string> {
                    {"server", "local" },
                    {"database", "db" },
                    {"port", "999" },
                    {"userid", "sa" },
                    {"password", "pwd" }
                })
            );
        }

        [Fact]
        public void MsSqlDataAccessLayer_NewInstanceWithConnectioAndUserNamePassword_ValidConnection()
        {
            var connection = new MsSqlDataAccessLayer(new Dictionary<string, string> {
                {"server", "local" },
                {"database", "db" },
                {"port", "999" },
                {"userid", "sa" },
                {"password", "pwd" }
            });
            Assert.True(connection.ValidConnection);
        }

        [Fact]
        public void MsSqlDataAccessLayer_NewInstanceWithConnectioAndUserNamePassword_InvalidConnection()
        {
            var builder = new MsSqlDataAccessLayer(new Dictionary<string, string> {
                {"port", "999" },
                {"userid", "sa" },
                {"password", "pwd" }
            });
            Assert.False(builder.ValidConnection);
        }

        [Fact]
        public async Task MsSqlDataAccessLayer_ReadDataSet()
        {
            var connection = new Dictionary<string, string> {
                {"server", "test-local" },
                {"database", "test-db" },
                {"port", "test-port" },
                {"userid", "test-sa" },
                {"password", "test-pwd" }
            };

            var wrapper = new Mock<SqlConnectionWrapper>("");
            var command = new Mock<SqlCommandWrapper>();
            wrapper
                .Setup(_ => _.CreateCommand())
                .Returns(command.Object);

            var factory = new Func<string, SqlConnectionWrapper>((s) => wrapper.Object);
            
            var builder = new MsSqlDataAccessLayer(factory, connection);

            var ds = await builder.SelectDataAsDataSet("select query");
            Assert.NotNull(ds);
            Assert.NotEmpty(ds);
            Assert.True(ds.ElementAtOrDefault(0)?.Columns?.ContainsKey("column-0"));
        }
    }
}