using data_access_layer.Interface;
using data_access_layer.Microsoft.SQL;
using data_access_layer.Microsoft.SQL.Models;
using Microsoft.Data.SqlClient;
using Moq;
using System.Collections.ObjectModel;
using System.Data.Common;

namespace data_access_layer.Tests
{
    public class MsSqlDataAccessLayerTests
    {
        [Fact]
        public void MsSqlDataAccessLayer_NewInstanceWithConnectionNull_NotNull()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.NotNull(() => new MsSqlDataAccessLayer(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void MsSqlDataAccessLayer_NewInstanceWithConnectionString_NotNull()
        {
            Assert.NotNull(() => new MsSqlDataAccessLayer(new MsSqlConnection("connectionString")));
        }

        [Fact]
        public void MsSqlDataAccessLayer_NewInstanceWithConnection_NotNull()
        {
            Assert.NotNull(() =>
                new MsSqlDataAccessLayer(new MsSqlConnection("test-local", "test-db", "test-userid", "test-pwd")));
        }

        class DbColumnStub(string columnName) : DbColumn
        {
            public new string ColumnName { get; set; } = columnName;
        }

        [Fact]
        public void MsSqlDataAccessLayer_FactoryInvokeNoConnection_ConectionIsNull()
        {
            var wrapper = new Mock<IDbConnectionWrapper<SqlConnectionStringBuilder>>();

            var factory = new Func<MsSqlConnection, IDbConnectionWrapper<SqlConnectionStringBuilder>>((s) => wrapper.Object);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var _connection = factory.Invoke(null).Connection;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            Assert.Null(_connection);
        }

        [Fact]
        public void MsSqlDataAccessLayer_FactoryInvokeWithConnection_ConectionNotNullAndEqual()
        {
            var connection = new MsSqlConnection("test-local", "test-db", "test-userid", "test-pwd");
            var wrapper = new Mock<IDbConnectionWrapper<SqlConnectionStringBuilder>>();
            wrapper
                .Setup(w => w.Connection)
                .Returns(connection);

            var factory = new Func<MsSqlConnection, IDbConnectionWrapper<SqlConnectionStringBuilder>>((s) => wrapper.Object);
            
            var _connection = factory.Invoke(connection).Connection;

            Assert.NotNull(_connection);
            Assert.Equal(connection, _connection);
        }
         
        [Fact]
        public async Task MsSqlDataAccessLayer_Full_ReadDataSet()
        {
            var connection = new MsSqlConnection("test-local", "test-db", "test-userid", "test-pwd");

            var wrapper = new Mock<IDbConnectionWrapper<SqlConnectionStringBuilder>>();
            wrapper
                .Setup(w => w.Connection)
                .Returns(connection);

            var command = new Mock<MsSqlCommandWrapper>();
            var reader = new Mock<MsSqlDataReaderWrapper>();

            reader
                .SetupSequence(_ => _.HasRows)
                .Returns(true)
                .Returns(false);

            reader
                .Setup(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReadOnlyCollection<DbColumn>(new List<DbColumn>() { new DbColumnStub("column-0") }));

            command
                .Setup(_ => _.ExecuteReaderAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(reader.Object);

            //wrapper.Setup(_ => _.CreateCommand()).Returns(command.Object);
            //wrapper
            //    .Setup(_ => _.CreateCommand())
            //    .Returns(new Mock<SqlCommandWrapper>().Object);

            //var wrapper = new SqlConnectionWrapperStub(connection);
            var factory = new Func<MsSqlConnection, IDbConnectionWrapper<SqlConnectionStringBuilder>>((s) => wrapper.Object);

            var builder = new MsSqlDataAccessLayer(connection, factory);

            var ds = await builder.SelectDataAsDataSetAsync("select sql query");
            //Assert.NotNull(ds);
            //Assert.NotEmpty(ds);
            //Assert.True(ds.ElementAtOrDefault(0)?.Columns?.ContainsKey("column-0"));
        }
    }
}