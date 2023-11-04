using data_access_layer.Microsoft.SQL;
using data_access_layer.Microsoft.SQL.Models;
using Moq;
using System.Data.Common;

namespace data_access_layer.Tests.Microsoft.SQL
{
    public class MsSqlDataAccessLayerTests
    {
        //private readonly Mock<IMsSqlDbFactory> factory = new();

        [Fact]
        public void NewInstanceWithConnectionNull_NotNull()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.NotNull(() => new MsSqlDataAccessLayer(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        //[Fact]
        //public void NewInstanceWithConnectionString_NotNull()
        //{
        //    Assert.NotNull(() => new MsSqlDataAccessLayer(new MsSqlConnectionWrapper("connectionString")));
        //}

        //[Fact]
        //public void NewInstanceWithConnection_NotNull()
        //{
        //    Assert.NotNull(() =>
        //        new MsSqlDataAccessLayer(new MsSqlConnectionWrapper("test-local", "test-db", "test-userid", "test-pwd")));
        //}

        //[Fact]
        //public async Task SelectDataAsDataSetAsync_EmptyQuery_EmptyDataSet()
        //{
        //    MsSqlDataAccessLayer dal = new(new MsSqlConnectionWrapper(""));

        //    var ds = await dal.RunSqlQueryAsDataSetAsync("");

        //    Assert.NotNull(ds);
        //    Assert.Empty(ds);
        //}

        [Fact]
        public async Task SelectDataAsDataSetAsync_NoConnection_EmptyDataSet()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            MsSqlDataAccessLayer dal = new(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            var ds = await dal.RunSqlQueryAsDataSetAsync("");

            Assert.NotNull(ds);
            Assert.Empty(ds);
        }

        [Fact]
        public async Task SelectDataAsDataSetAsync_InvalidConnection_EmptyDataSet()
        {
            //var connection = new Mock<MsSqlConnectionWrapper>("", factory.Object);

            ////connection
            ////    .Setup(f => f.IsValidConnection())
            ////    .Returns(false);

            //MsSqlDataAccessLayer dal = new(connection.Object);

            //var ds = await dal.RunSqlQueryAsDataSetAsync("");

            //Assert.NotNull(ds);
            //Assert.Empty(ds);
        }

        class DbColumnStub(string columnName) : DbColumn
        {
            public new string ColumnName { get; set; } = columnName;
        }

        [Fact]
        public async Task Full_ReadDataSet()
        {
            //factory
            //    .Setup(f => f.GetMsSqlDbConnection(new SqlConnectionStringBuilder()))
            //    .Returns(new Mock<DbConnection>().Object);

            //var connection = new MsSqlConnection("test-local", "test-db", "test-userid", "test-pwd", factory.Object);

            //var wrapper = new Mock<IDbConnectionWrapper<SqlConnectionStringBuilder>>();
            ////wrapper
            ////    .Setup(w => w.Connection)
            ////    .Returns(connection);

            //var command = new Mock<MsSqlCommandWrapper>();
            //var reader = new Mock<MsSqlDataReaderWrapper>();

            //reader
            //    .SetupSequence(_ => _.HasRows)
            //    .Returns(true)
            //    .Returns(false);

            //reader
            //    .Setup(_ => _.GetColumnSchemaAsync(It.IsAny<CancellationToken>()))
            //    .ReturnsAsync(new ReadOnlyCollection<DbColumn>(new List<DbColumn>() { new DbColumnStub("column-0") }));

            //command
            //    .Setup(_ => _.ExecuteReaderAsync(It.IsAny<CancellationToken>()))
            //    .ReturnsAsync(reader.Object);

            ////wrapper.Setup(_ => _.CreateCommand()).Returns(command.Object);
            ////wrapper
            ////    .Setup(_ => _.CreateCommand())
            ////    .Returns(new Mock<SqlCommandWrapper>().Object);

            ////var wrapper = new SqlConnectionWrapperStub(connection);
            ////var factory = new Func<MsSqlConnection, IDbConnectionWrapper<SqlConnectionStringBuilder>>((s) => wrapper.Object);

            //var builder = new MsSqlDataAccessLayer(connection);

            //var ds = await builder.SelectDataAsDataSetAsync("select sql query");
            ////Assert.NotNull(ds);
            ////Assert.NotEmpty(ds);
            ////Assert.True(ds.ElementAtOrDefault(0)?.Columns?.ContainsKey("column-0"));
        }
    }
}