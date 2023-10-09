using data_access_layer;

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
        public async Task ReadDataSet()
        {
            var access = new Dictionary<string, string>();
            MsSqlDataAccessLayer sql = new MsSqlDataAccessLayer(access);
            var result = await sql.SelectDataAsDataSet("");
        }
    }
}