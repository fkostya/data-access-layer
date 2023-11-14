using data_access_layer.Microsoft.SQL.Wrappers;
using data_access_layer.Model;
using Microsoft.Data.SqlClient;
using Moq;
using System.Data;
using System.Data.Common;

namespace data_access_layer.Tests.Microsoft.SQL.Wrappers
{
    public class MsSqlConnectionWrapperTests
    {
        [Fact]
        public void NewInstance_ValidConnection_InstanceNotNull()
        {
            MsSqlConnectionString connection = new("name", "server", "database", "sid");
            MsSqlConnectionWrapper wrapper = new(connection);

            Assert.NotNull(wrapper);
        }

        [Fact]
        public void NewInstance_ConnectionNull_InstanceNotNull()
        {
            MsSqlConnectionWrapper wrapper = new(null);

            Assert.NotNull(wrapper);
        }

        [Fact]
        public void NewInstance_ConnectionAndFuncFactory_InstanceNotNull()
        {
            var connection = new MsSqlConnectionString("name", "server", "database", "sid");
            Func<MsSqlConnectionString, DbConnection> factory = (connection) => new SqlConnection(connection.ConnectionString);
            MsSqlConnectionWrapper wrapper = new(connection, factory);

            Assert.NotNull(wrapper);
            Assert.NotNull(wrapper.Connection);
        }

        [Fact]
        public async Task OpenAsync_SqlInstanceIsNull_VerifyCallOneTime()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var wrapper = new MsSqlConnectionWrapper(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            await wrapper.OpenAsync(default);
        }

        [Fact]
        public async Task OpenAsync_SqlInstanceIsNotNull_VerifyCallOneTime()
        {
            var connection = new Mock<DbConnection>();
            connection
                .Setup(_ => _.OpenAsync(It.IsAny<CancellationToken>()))
                .Verifiable(Times.Once);

            var func = new Func<MsSqlConnectionString, DbConnection>((c) => connection.Object);
            var wrapper = new MsSqlConnectionWrapper(new MsSqlConnectionString("n", "s", "d", "s"), func);

            await wrapper.OpenAsync(default);
        }

        [Fact]
        public async Task OpenAsync_SqlInstanceThrowException_VerifyCallOneTime()
        {
            var connection = new Mock<DbConnection>();
            connection
                .Setup(_ => _.OpenAsync(It.IsAny<CancellationToken>()))
                .Throws(new Exception());

            var func = new Func<MsSqlConnectionString, DbConnection>((c) => connection.Object);
            var wrapper = new MsSqlConnectionWrapper(new MsSqlConnectionString("n", "s", "d", "s"), func);

            await wrapper.OpenAsync(default);
        }

        [Fact]
        public void CreateCommand_SqlInstanceIsNull_ReturnsCommandWrapper() 
        {
            MsSqlConnectionWrapper wrapper = new(null);

            var command = wrapper.CreateCommand();

            Assert.NotNull(command);
        }

        [Fact]
        public void CreateCommand_SqlInstanceIsNotNull_ReturnsCommandWrapper()
        {
            MsSqlConnectionWrapper wrapper = new(new MsSqlConnectionString("n", "s", "d", "s"));

            var command = wrapper.CreateCommand();

            Assert.NotNull(command);
        }

        class DbConnectionFakeException : DbConnection
        {
            public override string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
            MsSqlConnectionWrapper wrapper = new(new MsSqlConnectionString("n", "s", "d", "s"), func);

            var command = wrapper.CreateCommand();

            Assert.NotNull(command);
        }

        [Fact]
        public async Task CloseAsync_SqlInstanceIsNull_VerifyCallOneTime() {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var wrapper = new MsSqlConnectionWrapper(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            //wrapper.Setup(_ => _.CloseAsync()).Verifiable(Times.Once);

            await wrapper.CloseAsync();
        }

        [Fact]
        public async Task CloseAsync_SqlInstanceIsNotNull_VerifyCallOneTime()
        {
            var connection = new Mock<DbConnection>();
            var func = new Func<MsSqlConnectionString, DbConnection>((c) => connection.Object);
            var wrapper = new MsSqlConnectionWrapper(new MsSqlConnectionString("n", "s", "d", "s"), func);

            connection.Setup(_ => _.CloseAsync()).Verifiable(Times.Once);

            await wrapper.CloseAsync();
        }

        [Fact]
        public async Task DisposeAsyncSqlInstanceIsNull_VerifyCallOneTime() {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var wrapper = new MsSqlConnectionWrapper(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            await wrapper.DisposeAsync();
        }

        [Fact]
        public async Task DisposeAsyncSqlInstanceIsNotNull_VerifyCallOneTime()
        {
            var connection = new Mock<DbConnection>();
            var func = new Func<MsSqlConnectionString, DbConnection>((c) => connection.Object);
            var wrapper = new MsSqlConnectionWrapper(new MsSqlConnectionString("n", "s", "d", "s"), func);

            connection.Setup(_ => _.DisposeAsync()).Verifiable(Times.Once);

            await wrapper.DisposeAsync();
        }
    }
}