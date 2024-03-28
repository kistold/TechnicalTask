using Shared.Config;

namespace PixelService
{
    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection(RabbitMqConfig.RabbitMq));
            builder.Services.AddSingleton<IMessageSender, RabbitMqMessageSender>();
        }

        public static void RegisterMiddlewares(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
        }
    }
}
