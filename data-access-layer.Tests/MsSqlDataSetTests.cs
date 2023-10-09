using data_access_layer;
using Moq;
using System.Data.Common;
using System.Reflection;

namespace data_access_layer_tests
{
    public class MsSqlDataSetTests
    {
        [Fact]
        public void MsSqlDataSet_NewInstance_IsNotNull()
        {
            MsSqlDataSet ds = new MsSqlDataSet();
            Assert.NotNull(ds);
        }

        [Fact]
        public void MsSqlDataSet_AddRow_RowCountIsOne()
        {
            MsSqlDataSet ds = new MsSqlDataSet();
            ds.Add(new Dictionary<string, object> { { "row-0", new object() } });

            Assert.NotEmpty(ds[0]);
        }

        [Fact]
        public void MsSqlDataSet_AddColumn_RowCountIsOne()
        {
            MsSqlDataSet ds = new MsSqlDataSet();
            var mockDbColumn = new Mock<DbColumn>();

            var col = mockDbColumn.Object.GetType().GetProperty(nameof(mockDbColumn.Object.ColumnName), BindingFlags.Public | BindingFlags.Instance);
            col?.SetValue(mockDbColumn.Object, "column-0");

            ds.AddColumn(mockDbColumn.Object);

            Assert.NotEmpty(ds.Columns);
        }
    }
}
