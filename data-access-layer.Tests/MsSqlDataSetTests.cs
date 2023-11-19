using Moq;
using System.Data.Common;
using System.Reflection;

namespace data_access_layer.Tests
{
    public class MsSqlDataSetTests
    {
        [Fact]
        public void MsSqlDataSet_NewInstance_IsNotNull()
        {
            MsSqlDataSet ds = new();
            Assert.NotNull(ds);
        }

        [Fact]
        public void MsSqlDataSet_AddRow_RowCountIsOne()
        {
            MsSqlDataSet ds = new();
            ds.AddRow(new Dictionary<string, object> { { "row-0", new object() } });

            Assert.NotEmpty(ds[0]);
        }

        [Fact]
        public void MsSqlDataSet_AddColumn_RowCountIsOne()
        {
            MsSqlDataSet ds = new();
            var mockDbColumn = new Mock<DbColumn>();

            var col = mockDbColumn.Object.GetType().GetProperty(nameof(mockDbColumn.Object.ColumnName), BindingFlags.Public | BindingFlags.Instance);
            col?.SetValue(mockDbColumn.Object, "column-0");

            ds.AddColumn(mockDbColumn.Object);

            Assert.NotEmpty(ds.Columns);
        }

        [Fact]
        public void MsSqlDataSet_NewInstance_UniqueDsName()
        {
            MsSqlDataSet ds = new();
            Assert.NotEmpty(ds.DataSetName);
        }

        [Fact]
        public void Rows_AddOneRow_ReturnOneRow()
        {
            MsSqlDataSet ds = new();
            ds.AddRow(new Dictionary<string, object> { { "key-1", new object() } });
            Assert.NotEmpty(ds.Rows);
        }
    }
}
