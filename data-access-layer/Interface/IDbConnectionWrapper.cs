using data_access_layer.MsSql;

namespace data_access_layer.Interface
{
    public interface IDbConnectionWrapper<T>
    {
        IConnection<T> Connection { get; }

        Task OpenAsync(CancellationToken cancellationToken = default);

        Task CloseAsync();

        SqlCommandWrapper CreateCommand();
    }
}