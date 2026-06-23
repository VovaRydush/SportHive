namespace SportHive.FinalFeatures;

public sealed class StatisticFilter
{
    public string? Season { get; set; }
    public string? Sport { get; set; }
    public string? System { get; set; }
    public string? Level { get; set; }
    public string? SortBy { get; set; }
    public string? Direction { get; set; }
}

public sealed class StatisticSummaryDto
{
    public int Events { get; set; }
    public int Matches { get; set; }
    public int FinishedMatches { get; set; }
    public int LiveMatches { get; set; }
    public int UpcomingMatches { get; set; }
    public int Participants { get; set; }
    public int Teams { get; set; }
    public int Organizations { get; set; }
}

public sealed class LeaderboardRowDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string? Sport { get; set; }
    public string? OrganizationLogin { get; set; }
    public string? OrganizationName { get; set; }
    public int Played { get; set; }
    public int Finished { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int ScoreFor { get; set; }
    public int ScoreAgainst { get; set; }
    public int ScoreDiff { get; set; }
    public int Points { get; set; }
    public double WinRate { get; set; }
}

public sealed class StatisticsDashboardDto
{
    public StatisticFilter Filter { get; set; } = new();
    public StatisticSummaryDto Summary { get; set; } = new();
    public List<string> Seasons { get; set; } = new();
    public List<string> Sports { get; set; } = new();
    public List<LeaderboardRowDto> AthleteLeaders { get; set; } = new();
    public List<LeaderboardRowDto> TeamLeaders { get; set; } = new();
    public List<LeaderboardRowDto> OrganizationLeaders { get; set; } = new();
    public List<LeaderboardRowDto> JudgeLeaders { get; set; } = new();
    public List<object> RecentFinishedMatches { get; set; } = new();
}

public sealed class TeamUpdateRequest
{
    public string? NewTeamName { get; set; }
    public string? TypeSport { get; set; }
    public string? LoginTrainer { get; set; }
    public string? TeamPhoto { get; set; }
}

public sealed class TeamAthleteRequest
{
    public string AthleteLogin { get; set; } = "";
    public string AthleteStatus { get; set; } = "Active";
}

public sealed class OrganizationMemberRequest
{
    public string Login { get; set; } = "";
    public string Role { get; set; } = "";
}

public sealed class ApiResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int ChangedRows { get; set; }
}
