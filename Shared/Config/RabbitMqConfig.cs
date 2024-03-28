namespace Shared.Config
{
    public sealed class RabbitMqConfig
    {
        public const string RabbitMq = "RabbitMq";

        public required string Host {  get; set; }

        public required int Port { get; set; }

        public required string UserName { get; set; }

        public required string Password { get; set; }
    }
}
