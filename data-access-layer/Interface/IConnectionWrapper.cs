namespace data_access_layer.Interface
{
    public interface IConnectionWrapper<T>
    {
        T GetConnection();

        bool IsValidConnection();
    }
}