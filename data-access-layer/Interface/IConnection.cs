namespace data_access_layer.Interface
{
    public interface IConnection<T>
    {
        T GetConnection();

        Task<bool> IsValidAsync();
    }
}