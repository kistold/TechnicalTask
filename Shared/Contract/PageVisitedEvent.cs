using System.Diagnostics.CodeAnalysis;

namespace Shared.Contract
{
    public sealed class PageVisitedEvent
    {
        public PageVisitedEvent()
        { }

        [SetsRequiredMembers]
        public PageVisitedEvent(DateTime visitDateTime, string referer, string userAgent, string visitorIp)
        {
            VisitDateTime = visitDateTime;
            Referer = referer;
            UserAgent = userAgent;
            VisitorIp = visitorIp;
        }

        public required DateTime VisitDateTime { get; init; }

        public required string Referer { get; init; }

        public required string UserAgent { get; init; }

        public required string VisitorIp { get; init; }
    }
}
