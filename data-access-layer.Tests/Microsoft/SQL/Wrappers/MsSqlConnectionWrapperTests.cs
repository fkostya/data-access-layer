using AutoFixture;
using data_access_layer.Microsoft.SQL.Wrappers;
using data_access_layer.Model;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using System.Data;
using System.Data.Common;

namespace data_access_layer.Tests.Microsoft.SQL.Wrappers
{
    public class MsSqlConnectionWrapperTests
    {
        private readonly IFixture _fixture;
        private readonly MsSqlConnectionWrapper _sut;
        private readonly Mock<DbConnection> _connectionMock;
        
        public MsSqlConnectionWrapperTests()
        {
            _fixture = new Fixture();
            _connectionMock = _fixture.Freeze<Mock<DbConnection>>();
            _fixture.Register(() =>
                new MsSqlConnectionString(
                    _fixture.Create<string>(),
                    _fixture.Create<string>(),
                    _fixture.Create<string>(),
                    _fixture.Create<string>()));

            _fixture.Register(() => new SqlConnection(_fixture.Create<MsSqlConnectionString>().ConnectionString));
            _fixture.Register(() => new Func<MsSqlConnectionString, DbConnection>((c) => _fixture.Create<SqlConnection>()));
            _fixture.Register(() => new Func<MsSqlConnectionString, DbConnection>((c) => _connectionMock.Object));

            _sut = new MsSqlConnectionWrapper(_fixture.Create<MsSqlConnectionString>());
        }

        [Fact]
        public void NewInstance_ValidConnection_InstanceNotNull()
        {
            _sut.Should().NotBeNull();
        }

        [Fact]
        public void NewInstance_ConnectionNull_InstanceNotNull()
        {
            MsSqlConnectionWrapper wrapper = new(null);

            wrapper.Should().NotBeNull();
        }

        [Fact]
        public void NewInstance_ConnectionAndFuncFactory_InstanceNotNull()
        {
            MsSqlConnectionWrapper sut = new(_fixture.Create<MsSqlConnectionString>(), _fixture.Create<Func<MsSqlConnectionString, DbConnection>>());

            sut.Should().NotBeNull();
            sut.Connection.Should().NotBeNull();
        }

        [Fact]
        public async Task OpenAsync_SqlInstanceIsNull_VerifyCallOneTime()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var sut = new MsSqlConnectionWrapper(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            await sut.OpenAsync(default);
        }

        [Fact]
        public async Task OpenAsync_SqlInstanceIsNotNull_VerifyCallOneTime()
        {
            _connectionMock
                .Setup(_ => _.OpenAsync(It.IsAny<CancellationToken>()))
                .Verifiable(Times.Once);

            var sut = new MsSqlConnectionWrapper(_fixture.Create<MsSqlConnectionString>(), _fixture.Create<Func<MsSqlConnectionString, DbConnection>>());

            await sut.OpenAsync(default);
            _connectionMock.Verify(_ => _.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OpenAsync_SqlInstanceThrowException_VerifyCallOneTime()
        {
            _connectionMock
                .Setup(_ => _.OpenAsync(It.IsAny<CancellationToken>()))
                .Throws(new Exception());

            var sut = new MsSqlConnectionWrapper(_fixture.Create<MsSqlConnectionString>(), _fixture.Create<Func<MsSqlConnectionString, DbConnection>>());
            await sut.OpenAsync(default);

            _connectionMock.Verify(_ => _.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void CreateCommand_SqlInstanceIsNull_ReturnsCommandWrapper() 
        {
            MsSqlConnectionWrapper sut = new(null);

            var command = sut.CreateCommand();

            command.Should().NotBeNull();
        }

        [Fact]
        public void CreateCommand_SqlInstanceIsNotNull_ReturnsCommandWrapper()
        {
            MsSqlConnectionWrapper sut = new(_fixture.Create<MsSqlConnectionString>());

            var command = sut.CreateCommand();

            command.Should().NotBeNull();
        }

        class DbConnectionFakeException : DbConnection
        {
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
            public override string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

            public override string Database => throw new NotImplementedException();

            public override string DataSource => throw new NotImplementedException();

            public override string ServerVersion => throw new NotImplementedException();

            public override ConnectionState State => throw new NotImplementedException();

            public override void ChangeDatabase(string databaseName)
            {
                throw new NotImplementedException();
            }

            public override void Close()
            {
                throw new NotImplementedException();
            }

            public override void Open()
            {
                throw new NotImplementedException();
            }

            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            {
                throw new NotImplementedException();
            }

            protected override DbCommand CreateDbCommand()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void CreateCommand_SqlInstanceThrowException_ReturnsCommandWrapper()
        {
            var connection = new DbConnectionFakeException();

            var func = new Func<MsSqlConnectionString, DbConnection>((c) => connection);
            MsSqlConnectionWrapper sut = new(_fixture.Create<MsSqlConnectionString>(), func);

            var command = sut.CreateCommand();

            command.Should().NotBeNull();
        }

        [Fact]
        public async Task CloseAsync_SqlInstanceIsNull_VerifyCallOneTime() {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var sut = new MsSqlConnectionWrapper(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            await sut.CloseAsync();
        }

        [Fact]
        public async Task CloseAsync_SqlInstanceIsNotNull_VerifyCallOneTime()
        {
            _connectionMock.Setup(_ => _.CloseAsync());
            var sut = new MsSqlConnectionWrapper(_fixture.Create<MsSqlConnectionString>(), _fixture.Create<Func<MsSqlConnectionString, DbConnection>>());

            await sut.CloseAsync();
            _connectionMock.Verify(_ => _.CloseAsync(), Times.Once);
        }

        [Fact]
        public async Task DisposeAsyncSqlInstanceIsNull_VerifyCallOneTime() {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var sut = new MsSqlConnectionWrapper(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            await sut.DisposeAsync();
        }

        [Fact]
        public async Task DisposeAsyncSqlInstanceIsNotNull_VerifyCallOneTime()
        {
            _connectionMock.Setup(_ => _.DisposeAsync());
            var sut = new MsSqlConnectionWrapper(_fixture.Create<MsSqlConnectionString>(), _fixture.Create<Func<MsSqlConnectionString, DbConnection>>());

            await sut.DisposeAsync();
            _connectionMock.Verify(_ => _.DisposeAsync(), Times.Once);
        }
    }
}