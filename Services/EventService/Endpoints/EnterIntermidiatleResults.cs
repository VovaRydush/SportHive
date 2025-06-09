using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using  SportHive.Services.Interfaces;

namespace Events.Endpoints
{
    public static class EnterIntermidiatleResults
    {
        public static void EnterIntermidiatleResult(this IEndpointRouteBuilder route)
        {
            route.MapPatch("/set-cycling-race", async ([FromBody] List<CyclingRace> races, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.SetCyclingRace(races);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/set-swimming-results", async ([FromBody] List<AthleteSwimming> races, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.SetSwimmingResults(races);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/set-rowing-race", async ([FromBody] List<RowingRace> races, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.SetRowingRace(races);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/set-distance-running", async ([FromBody] List<DistanceRunning> races, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.SetDistanceRunning(races);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/set-team-score", async ([FromBody] TeamScoreDto scoreDto, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.SetTeamScore(scoreDto);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/update-chess-move", async ([FromBody] ChessMove move, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.UpdateChessMove(move);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/update-checkers-move", async ([FromBody] CheckersMove move, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.UpdateCheckersMove(move);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/set-board-winner", async ([FromBody] WinnerDto winner, [FromServices] ISetResultMatch enterData) =>
            {
                await enterData.SetWinnerInMatchBoard(winner);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/set-weightlifting-results", async ([FromBody] Weightlifting result, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.SetWeightliftingResults(result);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/set-struggle-winner", async ([FromBody] WinnerDto winner, [FromServices] ISetResultMatch enterData) =>
            {
                await enterData.SetWinnerInMatchStruggle(winner);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
            
            route.MapPatch("/set-ring-points", async ([FromBody] PointsIntBoxStruggle points, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.SetPointsBoxStruggleCort(points);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
           
        }
    }
}