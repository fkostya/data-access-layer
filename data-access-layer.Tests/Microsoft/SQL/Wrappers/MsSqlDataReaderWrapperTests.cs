using data_access_layer.Microsoft.SQL.Wrappers;
using Moq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace data_access_layer.Tests.Microsoft.SQL
{
    public class MsSqlDataReaderWrapperTests
    {
        private readonly Mock<DbDataReader> reader = new();

        public class DbColumnStub(string columnName) : DbColumn
        {
            public new string ColumnName { get; set; } = columnName;
        }

        [Fact]
        public async Task GetColumnSchemaAsync_NoReaderProvided_EmptyColumns()
        {
            MsSqlDataReaderWrapper wrapper = new();
            var columns = await wrapper.GetColumnSchemaAsync();

            Assert.Empty(columns);
        }

        [Fact]
        public async Task GetColumnSchemaAsync_DbReaderProvided_NotEmptyColumns()
        {
            reader
                .Setup(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReadOnlyCollection<DbColumn>(new List<DbColumn>() { new DbColumnStub("column-0") }));

            MsSqlDataReaderWrapper wrapper = new(reader.Object);
            var columns = await wrapper.GetColumnSchemaAsync();

            Assert.NotEmpty(columns);
            Assert.Equal("column-0", (columns?.FirstOrDefault() as DbColumnStub)?.ColumnName);
        }

        [Fact]
        public async Task GetColumnSchemaAsync_ThrowException_ReturnsEmptyColumn()
        {
            reader
                .Setup(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>()))
                .Throws(new Exception());

            MsSqlDataReaderWrapper wrapper = new(reader.Object);
            var columns = await wrapper.GetColumnSchemaAsync();

            Assert.Empty(columns);
        }

        [Fact]
        public async Task NextResultAsync_DbReaderProvided_HasNextResultTrue()
        {
            reader
                .Setup(_ => _.NextResultAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            MsSqlDataReaderWrapper wrapper = new(reader.Object);
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

            MsSqlDataReaderWrapper wrapper = new(reader.Object);
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

            MsSqlDataReaderWrapper wrapper = new();
            var next = await wrapper.NextResultAsync();

            Assert.False(next);
        }

        [Fact]
        public void GetSchemaTable_NoDbReaderProvided_EmptyDataTable()
        {
            reader
                .Setup(_ => _.GetSchemaTable())
                .Returns(new DataTable());

            MsSqlDataReaderWrapper wrapper = new();
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

            MsSqlDataReaderWrapper wrapper = new(reader.Object);
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

            MsSqlDataReaderWrapper wrapper = new();
            var read = await wrapper.ReadAsync();

            Assert.False(read);
        }

        [Fact]
        public async Task ReadAsync_DbReaderProvided_HasReadTrue()
        {
            reader
                .Setup(_ => _.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            MsSqlDataReaderWrapper wrapper = new(reader.Object);
            var read = await wrapper.ReadAsync();

            Assert.True(read);
        }

        [Fact]
        public void GetValue_NoDbReaderProvided_ObjectAsNull()
        {
            MsSqlDataReaderWrapper wrapper = new();
            var value = wrapper.GetValueOrDefault(It.IsAny<string>());

            Assert.Null(value);
        }

        [Fact]
        public void GetValue_NoDbReaderProvided_ObjectAsEmpty()
        {
            MsSqlDataReaderWrapper wrapper = new();
            var value = wrapper.GetValueOrDefault(It.IsAny<string>(), new object());

            Assert.NotNull(value);
        }

        class DbDataReaderFakeException : DbDataReader
        {
            public override object this[int ordinal] => throw new NotImplementedException();

            public override object this[string name] => throw new NotImplementedException();

            public override int Depth => throw new NotImplementedException();

            public override int FieldCount => throw new NotImplementedException();

            public override bool HasRows => throw new NotImplementedException();

            public override bool IsClosed => throw new NotImplementedException();

            public override int RecordsAffected => throw new NotImplementedException();

            public override bool GetBoolean(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override byte GetByte(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
            {
                throw new NotImplementedException();
            }

            public override char GetChar(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
            {
                throw new NotImplementedException();
            }

            public override string GetDataTypeName(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override DateTime GetDateTime(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override decimal GetDecimal(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override double GetDouble(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }

            [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)]
            public override Type GetFieldType(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override float GetFloat(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override Guid GetGuid(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override short GetInt16(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override int GetInt32(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override long GetInt64(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override string GetName(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override int GetOrdinal(string name)
            {
                throw new NotImplementedException();
            }

            public override string GetString(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override object GetValue(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override int GetValues(object[] values)
            {
                throw new NotImplementedException();
            }

            public override bool IsDBNull(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override bool NextResult()
            {
                throw new NotImplementedException();
            }

            public override bool Read()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void GetValue_GetValueThrowException_ObjectAsEmpty()
        {
            MsSqlDataReaderWrapper wrapper = new(new DbDataReaderFakeException());
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

            MsSqlDataReaderWrapper wrapper = new(reader.Object);
            var value = wrapper.GetValueOrDefault(It.IsAny<string>());

            Assert.NotNull(value);
        }

        [Fact]
        public void Close_NoDbReaderProvided_NoException()
        {
            MsSqlDataReaderWrapper wrapper = new();

            wrapper.Close();
        }

        [Fact]
        public void Close_DbReaderProvided_CloseIsCalledOnce()
        {
            reader
                .Setup(x => x.Close())
                .Verifiable();

            MsSqlDataReaderWrapper wrapper = new(reader.Object);

            wrapper.Close();
        }

        [Fact]
        public async Task DisposeAsync_NoDbReaderProvided_DisposeAsyncCalledOnce()
        {
            reader
                .Setup(x => x.DisposeAsync())
                .Verifiable();

            MsSqlDataReaderWrapper wrapper = new(reader.Object);

            await wrapper.DisposeAsync();
        }

        [Fact]
        public void HasRow_NoDbReaderProvided_NoRowsFalse()
        {
            MsSqlDataReaderWrapper wrapper = new();

            var noRows = wrapper.HasRows;

            Assert.False(noRows);
        }

        [Fact]
        public void HasRow_DbReaderProvided_HasRowsTrue()
        {
            reader
                .Setup(x => x.HasRows)
                .Returns(true);

            MsSqlDataReaderWrapper wrapper = new(reader.Object);

            var hasRows = wrapper.HasRows;

            Assert.True(hasRows);
        }

        [Fact]
        public void Index_NoDbReaderProvided_EmptyObject()
        {
            MsSqlDataReaderWrapper wrapper = new();

            var value = wrapper[It.IsAny<string>()];

            Assert.NotNull(value);
        }

        [Fact]
        public void Index_DbReaderProvided_EmptyObject()
        {
            reader
                .Setup(x => x[It.IsAny<string>()])
                .Returns(new object());

            MsSqlDataReaderWrapper wrapper = new(reader.Object);

            var value = wrapper[It.IsAny<string>()];

            Assert.NotNull(value);
        }
    }
}