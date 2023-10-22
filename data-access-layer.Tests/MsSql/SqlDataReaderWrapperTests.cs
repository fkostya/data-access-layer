using data_access_layer.MsSql;
using Moq;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;

namespace data_access_layer.Tests.MsSql
{
    public class SqlDataReaderWrapperTests
    {
        public class DbColumnStub(string columnName) : DbColumn
        {
            public new string ColumnName { get; set; } = columnName;
        }

        [Fact]
        public async Task GetColumnSchemaAsync_NoReaderProvided_EmptyColumns()
        {
            SqlDataReaderWrapper wrapper = new();
            var columns = await wrapper.GetColumnSchemaAsync();

            Assert.Empty(columns);
        }

        [Fact]
        public async Task GetColumnSchemaAsync_DbReaderProvided_NotEmptyColumns()
        {
            var reader = new Mock<DbDataReader>();

            reader
                .Setup(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReadOnlyCollection<DbColumn>(new List<DbColumn>() { new DbColumnStub("column-0") }));

            SqlDataReaderWrapper wrapper = new(reader.Object);
            var columns = await wrapper.GetColumnSchemaAsync();

            Assert.NotEmpty(columns);
            Assert.Equal("column-0", (columns?.FirstOrDefault() as DbColumnStub)?.ColumnName);
        }

        [Fact]
        public async Task NextResultAsync_DbReaderProvided_HasNextResultTrue()
        {
            var reader = new Mock<DbDataReader>();

            reader
                .Setup(_ => _.NextResultAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            SqlDataReaderWrapper wrapper = new(reader.Object);
            var next = await wrapper.NextResultAsync();

            Assert.True(next);
        }

        [Fact]
        public async Task NextResultAsync_DbReaderProvided_HasLastResultFalse()
        {
            var reader = new Mock<DbDataReader>();

            reader
                .SetupSequence(_ => _.NextResultAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            SqlDataReaderWrapper wrapper = new(reader.Object);
            var next = await wrapper.NextResultAsync();
            var last = await wrapper.NextResultAsync();

            Assert.False(last);
        }

        [Fact]
        public async Task NextResultAsync_NoDbReaderProvided_HasNextResultFalse()
        {
            var reader = new Mock<DbDataReader>();

            reader
                .Setup(_ => _.NextResultAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            SqlDataReaderWrapper wrapper = new(reader.Object);
            var next = await wrapper.NextResultAsync();

            Assert.False(next);
        }

        [Fact]
        public void GetSchemaTable_NoDbReaderProvided_EmptyDataTable()
        {
            var reader = new Mock<DbDataReader>();

            reader
                .Setup(_ => _.GetSchemaTable())
                .Returns(new DataTable());

            SqlDataReaderWrapper wrapper = new(reader.Object);
            var db = wrapper.GetSchemaTable();

            Assert.NotNull(db);
        }

        [Fact]
        public void GetSchemaTable_DbReaderProvided_NotEmptyDataTable()
        {
            var reader = new Mock<DbDataReader>();
            var table = new DataTable("unit-test-table-0");

            reader
                .Setup(_ => _.GetSchemaTable())
                .Returns(table);

            SqlDataReaderWrapper wrapper = new(reader.Object);
            var db = wrapper.GetSchemaTable();

            Assert.NotNull(db);
            Assert.Equal("unit-test-table-0", db.TableName);
        }

        [Fact]
        public async Task ReadAsync_NoDbReaderProvided_NoReadFalse()
        {
            var reader = new Mock<DbDataReader>();

            reader
                .Setup(_ => _.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            SqlDataReaderWrapper wrapper = new(reader.Object);
            var read = await wrapper.ReadAsync();

            Assert.False(read);
        }

        [Fact]
        public async Task ReadAsync_DbReaderProvided_HasReadTrue()
        {
            var reader = new Mock<DbDataReader>();

            reader
                .Setup(_ => _.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            SqlDataReaderWrapper wrapper = new(reader.Object);
            var read = await wrapper.ReadAsync();

            Assert.True(read);
        }

        [Fact]
        public void GetValue_NoDbReaderProvided_ObjectAsNull()
        {
            var reader = new Mock<DbDataReader>();

            SqlDataReaderWrapper wrapper = new(reader.Object);
            var value = wrapper.GetValueOrDefault(It.IsAny<string>());

            Assert.Null(value);
        }

        [Fact]
        public void GetValue_NoDbReaderProvided_ObjectAsEmpty()
        {
            var reader = new Mock<DbDataReader>();

            SqlDataReaderWrapper wrapper = new(reader.Object);
            var value = wrapper.GetValueOrDefault(It.IsAny<string>(), new object());

            Assert.NotNull(value);
        }

        //[Fact]
        //public void GetValue_DbReaderProvided_ObjectAsEmpty()
        //{
        //    var reader = new Mock<DbDataReader>();

        //    reader
        //        .Setup(_ => _.GetValue(It.IsAny<string>()))
        //        .Returns(new object());

        //    SqlDataReaderWrapper wrapper = new(reader.Object);
        //    var value = wrapper.GetValueOrDefault(It.IsAny<string>());

        //    Assert.NotNull(value);
        //}
    }
}