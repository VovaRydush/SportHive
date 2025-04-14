using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Command.Endpoints
{
    public static class TrainerService
    {
        public static void TrainerEndpoint(this IEndpointRouteBuilder route)
        {
          var TrainerRoute = route.MapGroup("trainer");

        }
    }
}
