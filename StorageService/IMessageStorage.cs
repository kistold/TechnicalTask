namespace StorageService
{
    public interface IMessageStorage<T>
    {
        void Store(T message);
    }
}
