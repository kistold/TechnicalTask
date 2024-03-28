namespace PixelService
{
    public interface IMessageSender
    {
        void SendMessage<T>(T message);
    }
}
