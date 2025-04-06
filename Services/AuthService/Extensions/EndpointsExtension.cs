using AuthService.Endpoints;

namespace SportHive.Extensions{
    public static class EndpointExtensions{
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app){
            app.UserRegisterEndpoint();
            app.UserLoginEndpoint();
        }
    }
}