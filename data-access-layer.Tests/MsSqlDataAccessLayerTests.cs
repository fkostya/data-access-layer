using data_access_layer;
using Microsoft.Data.SqlClient;
using Moq;

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
            MsSqlDataAccessLayer build = new(new Dictionary<string, string> {
                {"server", "local" },
                {"database", "db" },
                {"port", "999" },
                {"userid", "sa" },
                {"password", "pwd" }
            });

            var mock = new Mock<MsSqlDataAccessLayer>();
            var con = new SqlConnection();
            mock
                .Setup(f => f.GetConnection(It.IsAny<string>()))
                .Returns(con);
            var result = await build.SelectDataAsDataSet("select");
        }
    }
}