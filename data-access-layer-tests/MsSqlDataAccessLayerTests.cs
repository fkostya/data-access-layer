using data_access_layer;
using System.Text;

namespace data_access_layer_tests
{
    public class MsSqlDataAccessLayerTests
    {
        [Test]
        public void MsSqlDataAccessLayer_NewInstanceWithNull_NoException()
        {
            Assert.DoesNotThrow(() => new MsSqlDataAccessLayer(string.Empty));
        }

        [Test]
        public async Task ReadDataSet()
        {
            var access = new Dictionary<string, string>();
            MsSqlDataAccessLayer sql = new MsSqlDataAccessLayer(access);
            await sql.SelectDataAsDataSet("");
        }
    }
}