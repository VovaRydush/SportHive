using Command.Endpoints;

namespace Extensions
{
    public static class EndpointExtensions
    {
        public static void MapCommandEndpoints(this IEndpointRouteBuilder app)
        {
            app.CreateCommandEndpoint();
            app.GetInfoTeam();
            app.TrainerEndpoint();
            
        }
    }
}
