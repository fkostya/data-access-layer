using AutoFixture;
using data_access_layer.Microsoft.SQL.Wrappers;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using System.Data.Common;

namespace data_access_layer.Tests.Microsoft.SQL.Wrappers
{
    public class MsSqlCommandWrapperTests
    {
        class DbCommandFake
        {
            public readonly Mock<DbCommand> Mock;
            private readonly Mock<DbDataReader> readerMock;
            private readonly IFixture _fixture;

            public DbCommandFake()
            {
                _fixture = new Fixture();
                Mock = _fixture.Create<Mock<DbCommand>>();
                readerMock = _fixture.Create<Mock<DbDataReader>>();
            }

            public Task<DbDataReader> ExecuteReaderAsync()
            {
                return Task.FromResult(readerMock.Object);
            }
        }

        private readonly MsSqlCommandWrapper _sut;
        private readonly IFixture _fixture;
        private readonly DbCommandFake _commandFake;

        public MsSqlCommandWrapperTests()
        {
            _fixture = new Fixture();
            _commandFake = _fixture.Freeze<DbCommandFake>();
            _sut = new(_commandFake.Mock.Object);
            _fixture.Register(() => new SqlCommand());
        }

        [Fact]
        public void NewInstance_EmptyArgument_NotNull()
        {
            _sut.Should().NotBeNull();
        }

        [Fact]
        public void NewInstance_SqlCommandAsArgument_NotNull()
        {
            MsSqlCommandWrapper wrapper = new(_fixture.Create<SqlCommand>());
            wrapper.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteReaderAsync_SqlCommandAsArgument_NotNull()
        {
            var result = await _sut.ExecuteReaderAsync(CancellationToken.None);
            
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task DisposeAsync_MethodCalledOnce()
        {
            await _sut.DisposeAsync();
            _commandFake.Mock.Verify(_ => _.DisposeAsync(), Times.Once);
        }

        [Fact]
        public void CommandText_SetCommand_GetEqualToSet()
        {
            _commandFake.Mock.Setup(_ => _.CommandText).Returns("test command");
            _sut.CommandText = "test command";

            _sut.CommandText.Should().BeSameAs("test command");
        }
    }
}