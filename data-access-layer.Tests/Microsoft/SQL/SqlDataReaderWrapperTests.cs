using data_access_layer.Microsoft.SQL.Models;
using Moq;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;

namespace data_access_layer.Tests.Microsoft.SQL
{
    public class SqlDataReaderWrapperTests
    {
        private readonly Mock<DbDataReader> reader = new();

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
            reader
                .Setup(_ => _.NextResultAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            SqlDataReaderWrapper wrapper = new();
            var next = await wrapper.NextResultAsync();

            Assert.False(next);
        }

        [Fact]
        public void GetSchemaTable_NoDbReaderProvided_EmptyDataTable()
        {
            reader
                .Setup(_ => _.GetSchemaTable())
                .Returns(new DataTable());

            SqlDataReaderWrapper wrapper = new();
            var db = wrapper.GetSchemaTable();

            Assert.NotNull(db);
        }

        [Fact]
        public void GetSchemaTable_DbReaderProvided_NotEmptyDataTable()
        {
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
            reader
                .Setup(_ => _.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            SqlDataReaderWrapper wrapper = new();
            var read = await wrapper.ReadAsync();

            Assert.False(read);
        }

        [Fact]
        public async Task ReadAsync_DbReaderProvided_HasReadTrue()
        {
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
            SqlDataReaderWrapper wrapper = new();
            var value = wrapper.GetValueOrDefault(It.IsAny<string>());

            Assert.Null(value);
        }

        [Fact]
        public void GetValue_NoDbReaderProvided_ObjectAsEmpty()
        {
            SqlDataReaderWrapper wrapper = new();
            var value = wrapper.GetValueOrDefault(It.IsAny<string>(), new object());

            Assert.NotNull(value);
        }

        [Fact]
        public void GetValue_DbReaderProvided_ObjectAsEmpty()
        {
            reader
                .Setup(_ => _.GetValue(It.IsAny<int>()))
                .Returns(new object());
            reader
                .Setup(_ => _.GetOrdinal(It.IsAny<string>()))
                .Returns(0);

            SqlDataReaderWrapper wrapper = new(reader.Object);
            var value = wrapper.GetValueOrDefault(It.IsAny<string>());

            Assert.NotNull(value);
        }

        [Fact]
        public void Close_NoDbReaderProvided_NoException()
        {
            SqlDataReaderWrapper wrapper = new();
           
            wrapper.Close();
        }

        [Fact]
        public void Close_DbReaderProvided_CloseIsCalledOnce()
        {
            reader
                .Setup(x => x.Close())
                .Verifiable();

            SqlDataReaderWrapper wrapper = new(reader.Object);

            wrapper.Close();
        }

        [Fact]
        public async Task DisposeAsync_NoDbReaderProvided_DisposeAsyncCalledOnce()
        {
            reader
                .Setup(x => x.DisposeAsync())
                .Verifiable();

            SqlDataReaderWrapper wrapper = new();

            await wrapper.DisposeAsync();
        }

        [Fact]
        public void HasRow_NoDbReaderProvided_NoRowsFalse()
        {
            SqlDataReaderWrapper wrapper = new();

            var noRows = wrapper.HasRows;

            Assert.False(noRows);
        }

        [Fact]
        public void HasRow_DbReaderProvided_HasRowsTrue()
        {
            reader
                .Setup(x => x.HasRows)
                .Returns(true);

            SqlDataReaderWrapper wrapper = new(reader.Object);

            var hasRows = wrapper.HasRows;

            Assert.True(hasRows);
        }

        [Fact]
        public void Index_NoDbReaderProvided_EmptyObject()
        {
            SqlDataReaderWrapper wrapper = new();

            var value = wrapper[It.IsAny<string>()];

            Assert.NotNull(value);
        }

        [Fact]
        public void Index_DbReaderProvided_EmptyObject()
        {
            reader
                .Setup(x => x[It.IsAny<string>()])
                .Returns(new object());

            SqlDataReaderWrapper wrapper = new(reader.Object);

            var value = wrapper[It.IsAny<string>()];

            Assert.NotNull(value);
        }
    }
}