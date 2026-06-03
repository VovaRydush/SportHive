using Microsoft.AspNetCore.SignalR;

namespace SportHive.Hubs
{
    public class MatchLiveHub : Hub
    {
        public Task JoinMatch(string matchType, long matchId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, GroupName(matchType, matchId));
        }

        public Task LeaveMatch(string matchType, long matchId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(matchType, matchId));
        }

        public static string GroupName(string matchType, long matchId)
        {
            return $"match:{matchType.Trim().ToLower()}:{matchId}";
        }
    }
}
