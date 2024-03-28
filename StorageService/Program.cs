using Shared.Config;
using Shared.Contract;

namespace StorageService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            var filePath = builder.Configuration.GetValue<string>("FileMessageStoragePath");
            if (filePath == null)
                throw new Exception("Configuration error");
            builder.Services.AddSingleton<IMessageStorage<PageVisitedEvent>>(sp => new FileMessageStorage(filePath));

            builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection(RabbitMqConfig.RabbitMq));
            builder.Services.AddHostedService<RabbitMqReceiver<PageVisitedEvent>>();

            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
