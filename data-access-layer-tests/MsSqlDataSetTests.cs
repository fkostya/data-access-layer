using data_access_layer;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace data_access_layer_tests
{
    public class MsSqlDataSetTests
    {
        [Test]
        public void MsSqlDataSet_NewInstance_IsNotNull()
        {
            MsSqlDataSet ds = new MsSqlDataSet();
            Assert.IsNotNull(ds);
        }

        [Test]
        public void MsSqlDataSet_AddRow_RowCountIsOne()
        {
            MsSqlDataSet ds = new MsSqlDataSet();
            ds.Add(new Dictionary<string, object> { { "row-0", new object() } });

            Assert.True(1 == ds[0].Count());
        }

        [Test]
        public void MsSqlDataSet_AddColumn_RowCountIsOne()
        {
            MsSqlDataSet ds = new MsSqlDataSet();
            var mockDbColumn = new Mock<DbColumn>();

            var col = mockDbColumn.Object.GetType().GetProperty(nameof(mockDbColumn.Object.ColumnName), BindingFlags.Public | BindingFlags.Instance);
            col?.SetValue(mockDbColumn.Object, "column-0");

            ds.AddColumn(mockDbColumn.Object);

            Assert.True(1 == ds.Columns.Count());
        }
    }
}
