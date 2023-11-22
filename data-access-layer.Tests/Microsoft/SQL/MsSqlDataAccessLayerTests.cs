using AutoFixture;
using data_access_layer.Microsoft.SQL;
using data_access_layer.Microsoft.SQL.Wrappers;
using data_access_layer.Model;
using FluentAssertions;
using Moq;
using System.Collections.ObjectModel;
using System.Data.Common;

namespace data_access_layer.Tests.Microsoft.SQL
{
    public class MsSqlDataAccessLayerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<MsSqlConnectionWrapper> _connectionMock;
        private readonly MsSqlDataAccessLayer _sut;

        public MsSqlDataAccessLayerTests()
        {
            _fixture = new Fixture();
            _fixture.Register(() => new MsSqlConnectionString(
                                    _fixture.Create<string>(),
                                    _fixture.Create<string>(),
                                    _fixture.Create<string>(),
                                    _fixture.Create<string>()));
            _fixture.Register(() => new MsSqlConnectionWrapper(_fixture.Create<MsSqlConnectionString>()));

            _connectionMock = new Mock<MsSqlConnectionWrapper>(_fixture.Create<MsSqlConnectionString>());
            
            _sut = new(_connectionMock.Object);
        }

        [Fact]
        public void NewInstanceWithConnectionNull_NotNull()
        {
#pragma warning disable CS8603 // Possible null reference return.
            _fixture.Register<MsSqlConnectionWrapper>(() => null);
#pragma warning restore CS8603 // Possible null reference return.

            var instance = new MsSqlDataAccessLayer(_fixture.Create<MsSqlConnectionWrapper>());
            instance.Should().NotBeNull();
        }

        [Fact]
        public void NewInstanceWithConnectionString_NotNull()
        {
            var instance = new MsSqlDataAccessLayer(new MsSqlConnectionWrapper(_fixture.Create<MsSqlConnectionString>()));

            instance.Should().NotBeNull();
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_EmptyQuery_ReturnsEmptyDataSet()
        {
            var ds = await _sut.RunSqlQueryAsDataSetAsync("");

            ds.Should().NotBeNull();
            ds.Should().BeEmpty();
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_CreateCommandNull_ReturnsEmptyDataSet()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _connectionMock.Setup(_ => _.CreateCommand()).Returns<MsSqlCommandWrapper>(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            MsSqlDataAccessLayer layer = new(_connectionMock.Object);
            var ds = await layer.RunSqlQueryAsDataSetAsync(_fixture.Create<string>());

            ds.Should().NotBeNull();
            ds.Should().BeEmpty();

            _connectionMock.Verify(f => f.CreateCommand(), Times.Once);
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_ExecuteReaderAsyncNull_ReturnsEmptyDataSet()
        {
            var command = new Mock<MsSqlCommandWrapper>();

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            command.Setup(_ => _.ExecuteReaderAsync(It.IsAny<CancellationToken>())).ReturnsAsync(null as MsSqlDataReaderWrapper);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            _connectionMock.Setup(_ => _.CreateCommand()).Returns(command.Object);

            MsSqlDataAccessLayer layer = new(_connectionMock.Object);
            var ds = await layer.RunSqlQueryAsDataSetAsync(_fixture.Create<string>());

            ds.Should().NotBeNull();
            ds.Should().BeEmpty();

            command.Verify(_ => _.ExecuteReaderAsync(It.IsAny<CancellationToken>()), Times.Once);
            _connectionMock.Verify(_ => _.CreateCommand(), Times.Once);
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_HasRowsFalse_ReturnsEmptyDataSet()
        {
            var reader = new Mock<MsSqlDataReaderWrapper>();
            var command = new Mock<MsSqlCommandWrapper>();

            reader.Setup(_ => _.HasRows).Returns(false);
            command.Setup(_ => _.ExecuteReaderAsync(It.IsAny<CancellationToken>())).ReturnsAsync(reader.Object);
            _connectionMock.Setup(_ => _.CreateCommand()).Returns(command.Object);

            MsSqlDataAccessLayer layer = new(_connectionMock.Object);
            var ds = await layer.RunSqlQueryAsDataSetAsync(_fixture.Create<string>());

            ds.Should().NotBeNull();
            ds.Should().BeEmpty();

            reader.Verify(_ => _.HasRows, Times.Once);
            command.Verify(_ => _.ExecuteReaderAsync(It.IsAny<CancellationToken>()), Times.Once);
            _connectionMock.Verify(_ => _.CreateCommand(), Times.Once);
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_NoConnection_ReturnsEmptyDataSet()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            MsSqlDataAccessLayer dal = new(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            var ds = await _sut.RunSqlQueryAsDataSetAsync(_fixture.Create<string>());

            ds.Should().NotBeNull();
            ds.Should().BeEmpty();
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_InvalidConnection_ReturnsEmptyDataSet()
        {
            MsSqlDataAccessLayer layer = new(new MsSqlConnectionWrapper(
                new MsSqlConnectionString(_fixture.Create<string>(), "", "", _fixture.Create<string>())));

            var ds = await _sut.RunSqlQueryAsDataSetAsync(_fixture.Create<string>());

            ds.Should().NotBeNull();
            ds.Should().BeEmpty();
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_ReaderValueIsNull_ReturnsDataSetIsNull()
        {
            var command = new Mock<MsSqlCommandWrapper>();
            var reader = new Mock<MsSqlDataReaderWrapper>();
            var column = new Mock<DbColumn>();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            column.Object.GetType().GetProperty("ColumnName").SetValue(column.Object, "column-0");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var columns = new ReadOnlyCollection<DbColumn>(new List<DbColumn> {column.Object});

            reader.Setup(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>())).ReturnsAsync(columns);
            reader.Setup(_ => _.HasRows).Returns(true);
            reader.SetupSequence(_ => _.ReadAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

            command.Setup(_ => _.ExecuteReaderAsync(It.IsAny<CancellationToken>())).ReturnsAsync(reader.Object);

            _connectionMock.Setup(_ => _.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _connectionMock.Setup(_ => _.CreateCommand()).Returns(command.Object);

            MsSqlDataAccessLayer layer = new(_connectionMock.Object);
            var ds = await layer.RunSqlQueryAsDataSetAsync(_fixture.Create<string>());

            ds.Should().NotBeNull();
            ds.Should().NotBeEmpty();
            ds.ElementAt(0).Columns["column-0"].ColumnName.Should().BeSameAs("column-0");
            ds.ElementAt(0)[0].Should().NotBeEmpty();
            ds.ElementAt(0)[0]["column-0"].Should().BeNull();

            reader.Verify(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>()), Times.Once);
            reader.Verify(_ => _.HasRows, Times.Once);
            reader.Verify(_ => _.ReadAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));

            command.Verify(_ => _.ExecuteReaderAsync(It.IsAny<CancellationToken>()), Times.Once);

            _connectionMock.Verify(_ => _.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
            _connectionMock.Verify(_ => _.CloseAsync(), Times.Once);
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_ReaderValueNotNull_ReturnsDataSetNotNull()
        {
            var command = new Mock<MsSqlCommandWrapper>();
            var reader = new Mock<MsSqlDataReaderWrapper>();
            var column = new Mock<DbColumn>();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            column.Object.GetType().GetProperty("ColumnName").SetValue(column.Object, "column-0");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var columns = new ReadOnlyCollection<DbColumn>(new List<DbColumn> { column.Object });

            reader.Setup(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>())).ReturnsAsync(columns);
            reader.Setup(_ => _.HasRows).Returns(true);
            reader.SetupSequence(_ => _.ReadAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);
            reader.Setup(_ => _[It.IsAny<string>()]).Returns(new object());

            command.Setup(_ => _.ExecuteReaderAsync(It.IsAny<CancellationToken>())).ReturnsAsync(reader.Object);

            _connectionMock.Setup(_ => _.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _connectionMock.Setup(_ => _.CreateCommand()).Returns(command.Object);

            MsSqlDataAccessLayer layer = new(_connectionMock.Object);
            var ds = await layer.RunSqlQueryAsDataSetAsync(_fixture.Create<string>());

            ds.Should().NotBeNull();
            ds.Should().NotBeEmpty();
            ds.ElementAt(0).Columns["column-0"].ColumnName.Should().BeSameAs("column-0");
            ds.ElementAt(0)[0].Should().NotBeEmpty();
            ds.ElementAt(0)[0]["column-0"].Should().NotBeNull();

            reader.Verify(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>()), Times.Once);
            reader.Verify(_ => _.HasRows, Times.Once);
            reader.Verify(_ => _.ReadAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
            reader.Verify(_ => _[It.IsAny<string>()], Times.Once);
            
            command.Verify(_ => _.ExecuteReaderAsync(It.IsAny<CancellationToken>()), Times.Once);

            _connectionMock.Verify(_ => _.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
            _connectionMock.Verify(_ => _.CloseAsync(), Times.Once);
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_OpenAsyncThrowException_ReturnsEmptyDataSet()
        {
            var func = new Func<MsSqlConnectionString, DbConnection>((c) => new Mock<DbConnection>().Object);
            var mock = new Mock<MsSqlConnectionWrapper>(_fixture.Create<MsSqlConnectionString>(), func);

            mock.Setup(_ => _.OpenAsync(It.IsAny<CancellationToken>())).Throws(_fixture.Create<Exception>());

            var dal = new MsSqlDataAccessLayer(mock.Object);
            var ds = await dal.RunSqlQueryAsDataSetAsync(_fixture.Create<string>());

            ds.Should().NotBeNull();
            ds.Should().BeEmpty();

            mock.Verify(_ => _.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_GetColumnSchemaAsyncThrowException_ReturnsEmptyDataSetAndConnectionClosed()
        {
            var mock = new Mock<MsSqlConnectionWrapper>(_fixture.Create<MsSqlConnectionString>());
            var command = new Mock<MsSqlCommandWrapper>();
            var reader = new Mock<MsSqlDataReaderWrapper>();
            var column = new Mock<DbColumn>();

            mock.Setup(_ => _.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            mock.Setup(_ => _.CreateCommand()).Returns(command.Object);

            reader.Setup(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>())).Throws(_fixture.Create<Exception>());
            reader.Setup(_ => _.HasRows).Returns(true);

            command.Setup(_ => _.ExecuteReaderAsync(It.IsAny<CancellationToken>())).ReturnsAsync(reader.Object);

            MsSqlDataAccessLayer layer = new(mock.Object);
            var ds = await layer.RunSqlQueryAsDataSetAsync(_fixture.Create<string>());
            
            ds.Should().NotBeNull();
            ds.Should().BeEmpty();

            mock.Verify(_ => _.CloseAsync(), Times.Once);
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_ReadAsyncThrowException_ReturnsOneDataSetAndConnectionClosed()
        {
            var mock = new Mock<MsSqlConnectionWrapper>(_fixture.Create<MsSqlConnectionString>());
            var db = new Mock<DbConnection>();
            var command = new Mock<MsSqlCommandWrapper>();
            var reader = new Mock<MsSqlDataReaderWrapper>();
            var column = new Mock<DbColumn>();

            mock.Setup(_ => _.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            mock.Setup(_ => _.CreateCommand()).Returns(command.Object);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            column.Object.GetType().GetProperty("ColumnName").SetValue(column.Object, "column-0");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var columns = new ReadOnlyCollection<DbColumn>(new List<DbColumn> { column.Object });

            reader.Setup(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>())).ReturnsAsync(columns);
            reader.Setup(_ => _.HasRows).Returns(true);
            reader.SetupSequence(_ => _.ReadAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);
            reader.Setup(_ => _.NextResultAsync(It.IsAny<CancellationToken>())).Throws(_fixture.Create<Exception>());
            
            command.Setup(_ => _.ExecuteReaderAsync(It.IsAny<CancellationToken>())).ReturnsAsync(reader.Object);

            var func = new Func<MsSqlConnectionString, DbConnection>((c) => db.Object);

            MsSqlDataAccessLayer layer = new(mock.Object);
            var ds = await layer.RunSqlQueryAsDataSetAsync(_fixture.Create<string>());

            ds.Should().NotBeNull();
            ds.Should().NotBeEmpty();

            mock.Verify(_ => _.CloseAsync(), Times.Once);
        }
    }
}