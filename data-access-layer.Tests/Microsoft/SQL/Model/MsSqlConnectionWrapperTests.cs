using data_access_layer.Microsoft.SQL.Models;
using data_access_layer.Model;

namespace data_access_layer.Tests.Microsoft.SQL.Model
{
    public class MsSqlConnectionWrapperTests
    {
        [Fact]
        public void NewInstanceStringConnectionAsNull_NotNull()
        {
            MsSqlConnectionWrapper connection = new(null);
            Assert.NotNull(connection);
        }

        [Fact]
        public void NewInstanceStringConnectionAsParams_NotNull()
        {
            MsSqlConnectionWrapper connection = new(new MsSqlConnectionString("server", "database", "userid", "password"));
            Assert.NotNull(connection);
        }

        

        //[Fact]
        //public async Task OpenAsync_isValidConnectionStringFalse_TaskCompleted()
        //{
        //    MsSqlConnectionWrapper connection = new("");
        //    await connection.OpenAsync();

        //    //Assert.Equal(Task.CompletedTask);
        //}
    }
}