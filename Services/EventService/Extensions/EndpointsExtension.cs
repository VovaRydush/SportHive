using Events.Endpoints;

namespace Extensions
{
    public static class EndpointExtensions
    {
        public static void MapEventEndpoints(this IEndpointRouteBuilder app)
        {
            app.EventsEndpoints();
            app.CompliteInfoEndpoint();
            app.EnterIntermidiatleResult();
            app.UsersProfileEndpoint();
            app.Stage3TournamentEndpoint();
            app.EventCatalogEndpoint();
            app.SportRulesEndpoint();
        }
    }
}