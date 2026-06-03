namespace DB.SportHive.Domain
{
    public class EventCatalogItemDto
    {
        public long IdEvent { get; set; }
        public string NameEvent { get; set; } = null!;
        public string System { get; set; } = null!;
        public string TypeSport { get; set; } = null!;
        public DateTime DataStart { get; set; }
        public DateTime? DataEnd { get; set; }
        public string Description { get; set; } = null!;
        public string EventPhoto { get; set; } = null!;
        public string Status { get; set; } = "Upcoming";
        public int TotalMatches { get; set; }
        public int LiveMatches { get; set; }
        public int FinishedMatches { get; set; }
        public int UpcomingMatches { get; set; }
        public bool CanManageEvent { get; set; }
        public string AccessLevel { get; set; } = "View";
        public List<EventMatchCatalogDto> Matches { get; set; } = new();
        public List<EventStandingDto> Standings { get; set; } = new();
        public List<BracketRoundDto> Bracket { get; set; } = new();
    }

    public class EventMatchCatalogDto
    {
        public string MatchType { get; set; } = null!; // team | individual | extreme
        public long MatchId { get; set; }
        public long IdEvent { get; set; }
        public string FirstParticipant { get; set; } = null!;
        public string SecondParticipant { get; set; } = null!;
        public string? FirstLogin { get; set; }
        public string? SecondLogin { get; set; }
        public DateTime? DataMatch { get; set; }
        public TimeSpan? TimeMatch { get; set; }
        public int Tour { get; set; }
        public int? Group { get; set; }
        public string? LocationName { get; set; }
        public string Status { get; set; } = "Upcoming";
        public string? LoginJudge { get; set; }
        public string? AddInformation { get; set; }
        public string? Score { get; set; }
        public string? Winner { get; set; }
        public string? BracketCode { get; set; }
        public bool CanEdit { get; set; }
        public string AccessReason { get; set; } = "ViewOnly";
    }

    public class EventStandingDto
    {
        public string Participant { get; set; } = null!;
        public string? ParticipantLogin { get; set; }
        public int Played { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int Points { get; set; }
        public int ScoreFor { get; set; }
        public int ScoreAgainst { get; set; }
        public int ScoreDiff => ScoreFor - ScoreAgainst;
        public int Group { get; set; }
    }

    public class BracketRoundDto
    {
        public int Tour { get; set; }
        public int? Group { get; set; }
        public string BracketCode { get; set; } = "MAIN";
        public List<EventMatchCatalogDto> Matches { get; set; } = new();
    }

    public class EventCatalogQueryDto
    {
        public string? Search { get; set; }
        public string? TypeSport { get; set; }
        public string? System { get; set; }
        public string? Status { get; set; }
        public string? Login { get; set; }
        public string? Role { get; set; }
    }

    public class MatchResultSubmitDto
    {
        public string MatchType { get; set; } = null!;
        public long MatchId { get; set; }
        public string? Login { get; set; }
        public string? Role { get; set; }
        public string Score { get; set; } = null!;
        public string? Winner { get; set; }
        public string? Notes { get; set; }
        public bool FinishMatch { get; set; } = true;
    }
}
