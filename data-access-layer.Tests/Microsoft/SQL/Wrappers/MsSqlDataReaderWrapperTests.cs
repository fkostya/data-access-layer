using AutoFixture;
using data_access_layer.Microsoft.SQL.Wrappers;
using FluentAssertions;
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
        private readonly Mock<DbDataReader> reader;
        private readonly MsSqlDataReaderWrapper _sut;
        private readonly IFixture _fixture;
        
        public class DbColumnStub(string columnName) : DbColumn
        {
            public new string ColumnName { get; set; } = columnName;
        }

        public MsSqlDataReaderWrapperTests()
        {
            _fixture = new Fixture();
            reader = _fixture.Create<Mock<DbDataReader>>();
            _sut = _fixture.Create<MsSqlDataReaderWrapper>();
            _fixture
                .Register(() => new DataTable("unit-test-table-0"));
        }

        [Fact]
        public async Task GetColumnSchemaAsync_NoReaderProvided_EmptyColumns()
        {
            var columns = await _sut.GetColumnSchemaAsync();

            columns.Should().BeEmpty();
        }

        [Fact]
        public async Task GetColumnSchemaAsync_DbReaderProvided_NotEmptyColumns()
        {
            reader
                .Setup(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReadOnlyCollection<DbColumn>(new List<DbColumn>() { new DbColumnStub("column-0") }));

            MsSqlDataReaderWrapper sut = new(reader.Object);
            var columns = await sut.GetColumnSchemaAsync();

            columns.Should().NotBeEmpty();
            (columns?.FirstOrDefault() as DbColumnStub)?.ColumnName.Should().BeSameAs("column-0");
            reader.Verify(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetColumnSchemaAsync_ThrowException_ReturnsEmptyColumn()
        {
            reader
                .Setup(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>()))
                .Throws(new Exception());

            MsSqlDataReaderWrapper sut = new(reader.Object);
            var columns = await sut.GetColumnSchemaAsync();

            columns.Should().BeEmpty();
            reader.Verify(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task NextResultAsync_DbReaderProvided_HasNextResultTrue()
        {
            reader
                .Setup(_ => _.NextResultAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            MsSqlDataReaderWrapper sut = new(reader.Object);
            var next = await sut.NextResultAsync();

            next.Should().BeTrue();
            reader.Verify(_ => _.NextResultAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task NextResultAsync_DbReaderProvided_HasLastResultFalse()
        {
            reader
                .SetupSequence(_ => _.NextResultAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            MsSqlDataReaderWrapper sut = new(reader.Object);
            var next = await sut.NextResultAsync();
            var last = await sut.NextResultAsync();

            last.Should().BeFalse();
            reader.Verify(_ => _.NextResultAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task NextResultAsync_NoDbReaderProvided_HasNextResultFalse()
        {
            var next = await _sut.NextResultAsync();

            next.Should().BeFalse();
        }

        [Fact]
        public void GetSchemaTable_NoDbReaderProvided_EmptyDataTable()
        {
            var db = _sut.GetSchemaTable();

            db.Should().NotBeNull();
        }

        [Fact]
        public void GetSchemaTable_DbReaderProvided_NotEmptyDataTable()
        {
            var table = _fixture.Create<DataTable>();

            reader
                .Setup(_ => _.GetSchemaTable())
                .Returns(table);

            MsSqlDataReaderWrapper sut = new(reader.Object);
            var db = sut.GetSchemaTable();

            db.Should().NotBeNull();
            db?.TableName.Should().BeSameAs("unit-test-table-0");
            reader.Verify(_ => _.GetSchemaTable(), Times.Once);
        }

        [Fact]
        public async Task ReadAsync_NoDbReaderProvided_NoReadFalse()
        {
            reader
                .Setup(_ => _.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            MsSqlDataReaderWrapper sut = new(reader.Object);
            var read = await sut.ReadAsync();

            read.Should().BeFalse();
            reader.Verify(_ => _.ReadAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReadAsync_DbReaderProvided_HasReadTrue()
        {
            reader
                .Setup(_ => _.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            MsSqlDataReaderWrapper sut = new(reader.Object);
            var read = await sut.ReadAsync();

            read.Should().BeTrue();
            reader.Verify(_ => _.ReadAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void GetValue_NoDbReaderProvided_ObjectAsNull()
        {
            var value = _sut.GetValueOrDefault(It.IsAny<string>());

            value.Should().BeNull();
        }

        [Fact]
        public void GetValue_NoDbReaderProvided_ObjectAsEmpty()
        {
            var value = _sut.GetValueOrDefault(It.IsAny<string>(), new object());

            value.Should().NotBeNull();
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
            MsSqlDataReaderWrapper sut = new(_fixture.Create<DbDataReaderFakeException>());
            var value = sut.GetValueOrDefault(It.IsAny<string>(), new object());

            value.Should().NotBeNull();
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

            MsSqlDataReaderWrapper sut = new(reader.Object);
            var value = sut.GetValueOrDefault(It.IsAny<string>());

            value.Should().NotBeNull();
            reader.Verify(_ => _.GetValue(It.IsAny<int>()), Times.Once);
            reader.Verify(_ => _.GetOrdinal(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Close_NoDbReaderProvided_NoException()
        {
            _sut.Close();
        }

        [Fact]
        public void Close_DbReaderProvided_CloseIsCalledOnce()
        {
            reader
                .Setup(x => x.Close());

            MsSqlDataReaderWrapper sut = new(reader.Object);

            sut.Close();
            reader.Verify(_ => _.Close(), Times.Once);
        }

        [Fact]
        public async Task DisposeAsync_NoDbReaderProvided_DisposeAsyncCalledOnce()
        {
            reader
                .Setup(x => x.DisposeAsync());

            MsSqlDataReaderWrapper sut = new(reader.Object);

            await sut.DisposeAsync();
            reader.Verify(_ => _.DisposeAsync(), Times.Once);
        }

        [Fact]
        public void HasRow_NoDbReaderProvided_NoRowsFalse()
        {
            var noRows = _sut.HasRows;

            noRows.Should().BeFalse();
        }

        [Fact]
        public void HasRow_DbReaderProvided_HasRowsTrue()
        {
            reader
                .Setup(x => x.HasRows)
                .Returns(true);

            MsSqlDataReaderWrapper sut = new(reader.Object);

            var hasRows = sut.HasRows;

            hasRows.Should().BeTrue();
            reader.Verify(_ => _.HasRows, Times.Once);
        }

        [Fact]
        public void Index_NoDbReaderProvided_EmptyObject()
        {
            var value = _sut[It.IsAny<string>()];

            value.Should().NotBeNull();
        }

        [Fact]
        public void Index_DbReaderProvided_EmptyObject()
        {
            reader
                .Setup(x => x[It.IsAny<string>()])
                .Returns(new object());

            MsSqlDataReaderWrapper sut = new(reader.Object);

            var value = sut[It.IsAny<string>()];

            value.Should().NotBeNull();
            reader.Verify(_ => _[It.IsAny<string>()], Times.Once);
        }
    }
}