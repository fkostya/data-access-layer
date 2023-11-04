using data_access_layer.Microsoft.SQL.Wrappers;
using Microsoft.Data.SqlClient;
using Moq;
using System.Data.Common;

namespace data_access_layer.Tests.Microsoft.SQL.Wrappers
{
    public class MsSqlCommandWrapperTests
    {
        class DbCommandFake
        {
            public Mock<DbCommand> Mock { get; init; } = new Mock<DbCommand>();
            private Mock<DbDataReader> readerMock = new Mock<DbDataReader>();

            public Task<DbDataReader> ExecuteReaderAsync()
            {
                return Task.FromResult(readerMock.Object);
            }
        }

        [Fact]
        public void NewInstance_EmptyArgument_NotNull()
        {
            MsSqlCommandWrapper wrapper = new();
            Assert.NotNull(wrapper);
        }

        [Fact]
        public void NewInstance_SqlCommandAsArgument_NotNull()
        {
            MsSqlCommandWrapper wrapper = new(new SqlCommand());
            Assert.NotNull(wrapper);
        }

        [Fact]
        public async Task ExecuteReaderAsync_SqlCommandAsArgument_NotNull()
        {
            var mock = new DbCommandFake();

            MsSqlCommandWrapper wrapper = new(mock.Mock.Object);
            var result = await wrapper.ExecuteReaderAsync(CancellationToken.None);
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DisposeAsync_MethodCalledOnce()
        {
            var mock = new DbCommandFake();
            var wrapper = new Mock<MsSqlCommandWrapper>(mock.Mock.Object);
            
            await wrapper.Object.DisposeAsync();
        }

        [Fact]
        public void CommandText_SetCommand_GetEqualToSetS()
        {
            var mock = new DbCommandFake();
            var wrapper = new MsSqlCommandWrapper(mock.Mock.Object);
            wrapper.CommandText = "command test";

            Assert.Equal("command test", wrapper.CommandText);
        }
    }
}