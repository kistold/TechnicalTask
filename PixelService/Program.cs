namespace PixelService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.RegisterServices();

            var app = builder.Build();
            app.RegisterMiddlewares();
            app.RegisterEndpoint();

            await app.RunAsync();
        }
    }
}
