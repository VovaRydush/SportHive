namespace DB.SportHive.Domain
{
    public class Stage3CreateEventDto
    {
        public string NameEvent { get; set; } = null!;
        public SelectionSystems Systems { get; set; }
        public string TypeSport { get; set; } = null!;
        public string ParticipantType { get; set; } = "team"; // team | individual | extreme
        public List<string> Participants { get; set; } = new();
        public DateTime DataStart { get; set; } = DateTime.UtcNow;
        public DateTime? DataEnd { get; set; }
        public string? LoginJudge { get; set; }
        public string? Description { get; set; }
        public bool GenerateMatches { get; set; } = true;
    }

    public class Stage3GenerateMatchesDto
    {
        public string NameEvent { get; set; } = null!;
        public string ParticipantType { get; set; } = "team";
        public List<string> Participants { get; set; } = new();
        public string? LoginJudge { get; set; }
    }

    public class Stage3SetMatchResultDto
    {
        public string MatchType { get; set; } = "team"; // team | individual | extreme
        public long IdMatch { get; set; }
        public int ScoreEntity1 { get; set; }
        public int ScoreEntity2 { get; set; }
        public string? Winner { get; set; }
        public bool IsDraw { get; set; }
        public bool CloseMatch { get; set; } = true;
        public string? Comment { get; set; }
    }

    public class Stage3ChangeMatchStatusDto
    {
        public string MatchType { get; set; } = "team";
        public long IdMatch { get; set; }
        public StatusMatch Status { get; set; }
    }

    public class Stage3EventWorkspaceDto
    {
        public long IdEvent { get; set; }
        public string NameEvent { get; set; } = null!;
        public string TypeSport { get; set; } = null!;
        public SelectionSystems System { get; set; }
        public string SystemName => System.ToString();
        public DateTime DataStart { get; set; }
        public DateTime? DataEnd { get; set; }
        public string Description { get; set; } = "";
        public List<Stage3MatchDto> Matches { get; set; } = new();
        public List<Stage3StandingDto> Standings { get; set; } = new();
        public Stage3EventSummaryDto Summary { get; set; } = new();
    }

    public class Stage3EventSummaryDto
    {
        public int ParticipantsCount { get; set; }
        public int MatchesCount { get; set; }
        public int FinishedMatches { get; set; }
        public int LiveMatches { get; set; }
        public int UpcomingMatches { get; set; }
        public string? Winner { get; set; }
    }

    public class Stage3MatchDto
    {
        public long IdMatch { get; set; }
        public long IdEvent { get; set; }
        public string MatchType { get; set; } = null!;
        public string Entity1 { get; set; } = null!;
        public string Entity2 { get; set; } = null!;
        public int Tour { get; set; }
        public int? Group { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? DataMatch { get; set; }
        public TimeSpan? TimeMatch { get; set; }
        public string? LocationName { get; set; }
        public string? LoginJudge { get; set; }
        public string? AddInformation { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public string? Winner { get; set; }
        public bool Played { get; set; }
        public bool CanEnterResult { get; set; } = true;
    }

    public class Stage3StandingDto
    {
        public string Name { get; set; } = null!;
        public int Played { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int ScoreFor { get; set; }
        public int ScoreAgainst { get; set; }
        public int ScoreDiff => ScoreFor - ScoreAgainst;
        public int Points { get; set; }
    }
}
