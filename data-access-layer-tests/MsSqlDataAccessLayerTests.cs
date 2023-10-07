using data_access_layer;

namespace data_access_layer_tests
{
    public class MsSqlDataAccessLayerTests
    {
        [Test]
        public void MsSqlDataAccessLayer_NewInstanceWithNull_NoException()
        {
            Assert.DoesNotThrow(() => new MsSqlDataAccessLayer(null));
        }

        [Test]
        public async Task ReadDataSet()
        {
            var access = new Dictionary<string, object>();
            MsSqlDataAccessLayer sql = new MsSqlDataAccessLayer(access);
            await sql.ReadDataSet(string.Empty);
        }
    }
}