using AutoFixture;
using FluentAssertions;
using Moq;
using System.Data.Common;
using System.Reflection;

namespace data_access_layer.Tests
{
    public class MsSqlDataSetTests
    {
        private readonly IFixture _fixture;
        private readonly MsSqlDataSet _sut;

        public MsSqlDataSetTests()
        {
            _fixture = new Fixture();
            _sut = new();
        }

        [Fact]
        public void MsSqlDataSet_NewInstance_IsNotNull()
        {
            _sut.Should().NotBeNull();
        }

        [Fact]
        public void MsSqlDataSet_AddRow_RowCountIsOne()
        {
            _sut.AddRow(_fixture.Build<Dictionary<string, object>>()
                        .Do(x => x.Add("row-0", _fixture.Freeze<object>()))
                        .Create());

            _sut[0].Should().NotBeEmpty();
            _sut[0]["row-0"].Should().BeAssignableTo<object>();
        }

        [Fact]
        public void MsSqlDataSet_AddColumn_RowCountIsOne()
        {
            var mockDbColumn = new Mock<DbColumn>();

            var col = mockDbColumn.Object.GetType().GetProperty(nameof(mockDbColumn.Object.ColumnName), BindingFlags.Public | BindingFlags.Instance);
            col?.SetValue(mockDbColumn.Object, "column-0");

            _sut.AddColumn(mockDbColumn.Object);

            _sut.Columns.Should().NotBeEmpty();
        }

        [Fact]
        public void MsSqlDataSet_NewInstance_UniqueDsName()
        {
            _sut.DataSetName.Should().NotBeEmpty();
        }

        [Fact]
        public void RowsElementAt_AddOneRow_ReturnOneRow()
        {
            _sut.AddRow(_fixture.Build<Dictionary<string, object>>()
                        .Do(x => x.Add("key-0", _fixture.Freeze<object>()))
                        .Create());
            
            _sut.Rows.Should().NotBeEmpty();
            _sut.Rows[0].ElementAt(0).Key.Should().BeSameAs("key-0");
            _sut.Rows[0].ElementAt(0).Value.Should().BeAssignableTo<object>();
        }
    }
}