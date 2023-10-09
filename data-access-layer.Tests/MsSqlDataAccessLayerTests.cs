using data_access_layer;
using System.Text;

namespace data_access_layer_tests
{
    public class MsSqlDataAccessLayerTests
    {
        [Fact]
        public void MsSqlDataAccessLayer_NewInstanceWithNull_NoException()
        {
            //Assert.(() => new MsSqlDataAccessLayer(string.Empty));
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