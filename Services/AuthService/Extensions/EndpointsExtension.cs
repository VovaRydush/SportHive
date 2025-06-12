using AuthService.Endpoints;

namespace AuthService.Extensions{
    public static class EndpointExtensions{
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.UserRegisterEndpoint();
            app.UserLoginEndpoint();
            app.UserManipuleteEndpoint();
            app.UserProfileEndpoint();
        }
    }
}