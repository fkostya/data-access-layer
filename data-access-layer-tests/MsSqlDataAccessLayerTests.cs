using data_access_layer;
using System.Text;

namespace data_access_layer_tests
{
    public class MsSqlDataAccessLayerTests
    {
        [Test]
        public void MsSqlDataAccessLayer_NewInstanceWithNull_NoException()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.DoesNotThrow(() => new MsSqlDataAccessLayer(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public async Task ReadDataSet()
        {
            var access = new Dictionary<string, string>();
            MsSqlDataAccessLayer sql = new MsSqlDataAccessLayer(access);
            await sql.SelectDataAsDataSet(new StringBuilder());
        }
    }
}