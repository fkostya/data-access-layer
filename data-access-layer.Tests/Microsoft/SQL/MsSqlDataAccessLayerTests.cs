using data_access_layer.Microsoft.SQL;
using data_access_layer.Microsoft.SQL.Models;
using data_access_layer.Microsoft.SQL.Wrappers;
using data_access_layer.Model;
using Moq;
using System.Collections.ObjectModel;
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

        [Fact]
        public void NewInstanceWithConnectionString_NotNull()
        {
            Assert.NotNull(() => new MsSqlDataAccessLayer(
                new MsSqlConnectionWrapper(
                    new MsSqlConnectionString("n", "s", "d", "s"))));
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_EmptyQuery_EmptyDataSet()
        {
            MsSqlDataAccessLayer layer = new(new MsSqlConnectionWrapper(
                new MsSqlConnectionString("n", "s", "d", "s")));

            var ds = await layer.RunSqlQueryAsDataSetAsync("");

            Assert.NotNull(ds);
            Assert.Empty(ds);
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_NoConnection_EmptyDataSet()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            MsSqlDataAccessLayer dal = new(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            var ds = await dal.RunSqlQueryAsDataSetAsync("");

            Assert.NotNull(ds);
            Assert.Empty(ds);
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_InvalidConnection_EmptyDataSet()
        {
            MsSqlDataAccessLayer layer = new(new MsSqlConnectionWrapper(
                new MsSqlConnectionString("n", "", "", "s")));

            var ds = await layer.RunSqlQueryAsDataSetAsync("");

            Assert.NotNull(ds);
            Assert.Empty(ds);
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_ReaderValueIsNull_CheckValueIsNull()
        {
            var mock = new Mock<MsSqlConnectionWrapper>(new MsSqlConnectionString("n", "u", "p", "s"));
            var db = new Mock<DbConnection>();
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

            mock.Setup(_ => _.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            mock.Setup(_ => _.CreateCommand()).Returns(command.Object);

            var func = new Func<MsSqlConnectionString, DbConnection>((c) => db.Object);

            MsSqlDataAccessLayer layer = new(mock.Object);
            var ds = await layer.RunSqlQueryAsDataSetAsync("select test sql query");

            Assert.NotNull(ds);
            Assert.NotEmpty(ds);
            Assert.Equal("column-0", ds.ElementAt(0).Columns["column-0"].ColumnName);

            Assert.NotEmpty(ds.ElementAt(0)[0]);

            Assert.Null(ds.ElementAt(0)[0]["column-0"]);
            mock.Verify(_ => _.CloseAsync(), Times.Once);
        }

        [Fact]
        public async Task RunSqlQueryAsDataSetAsync_ReaderValueNotNull_CheckValueIsNotNull()
        {
            var mock = new Mock<MsSqlConnectionWrapper>(new MsSqlConnectionString("n", "u", "p", "s"));
            var db = new Mock<DbConnection>();
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

            mock.Setup(_ => _.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            mock.Setup(_ => _.CreateCommand()).Returns(command.Object);

            var func = new Func<MsSqlConnectionString, DbConnection>((c) => db.Object);

            MsSqlDataAccessLayer layer = new(mock.Object);
            var ds = await layer.RunSqlQueryAsDataSetAsync("select test sql query");

            Assert.NotNull(ds);
            Assert.NotEmpty(ds);
            Assert.Equal("column-0", ds.ElementAt(0).Columns["column-0"].ColumnName);

            Assert.NotEmpty(ds.ElementAt(0)[0]);

            Assert.NotNull(ds.ElementAt(0)[0]["column-0"]);
            mock.Verify(_ => _.CloseAsync(), Times.Once);
        }
    }
}