using Events.Endpoints;

namespace Extensions
{
    public static class EndpointExtensions
    {
        public static void MapEventEndpoints(this IEndpointRouteBuilder app)
        {
           app.EventsEndpoints();
        }
    }
}