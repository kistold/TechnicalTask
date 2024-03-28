using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Net.Http.Headers;
using Shared.Contract;

namespace PixelService
{
    public static class Endpoint
    {
        public static void RegisterEndpoint(this WebApplication app)
        {
            app
                .MapGet("/track", GetTrackingGif)
                .WithName(nameof(GetTrackingGif))
                .WithOpenApi();
        }

        private static FileContentHttpResult GetTrackingGif(HttpContext httpContext, IMessageSender messageSender)
        {
            var pageVisitedEvent = new PageVisitedEvent
            {
                VisitDateTime = DateTime.UtcNow,
                Referer = httpContext.Request.Headers[HeaderNames.Referer].ToString(),
                UserAgent = httpContext.Request.Headers[HeaderNames.UserAgent].ToString(),
                VisitorIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            };

            messageSender.SendMessage(pageVisitedEvent);

            return TypedResults.File(TrackingGif, "image/gif");
        }

        private static readonly byte[] TrackingGif = [0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x1, 0x0, 0x1, 0x0, 0x0, 0x0, 0x0, 0x21, 0xf9, 0x04, 0x1, 0x0, 0x0, 0x0, 0x0, 0x2c, 0x0, 0x0, 0x0, 0x0, 0x1, 0x0, 0x1, 0x0, 0x0, 0x2, 0x1, 0x0, 0x0];
    }
}
