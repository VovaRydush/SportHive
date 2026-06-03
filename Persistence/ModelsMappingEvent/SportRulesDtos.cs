namespace DB.SportHive.Domain
{
    public class SportRuleDto
    {
        public string Sport { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string MatchMode { get; set; } = "Team";
        public List<string> LiveEvents { get; set; } = new();
        public List<string> ResultFields { get; set; } = new();
        public List<string> ScoreExamples { get; set; } = new();
        public Dictionary<string, string> FieldLabels { get; set; } = new();
        public Dictionary<string, string> EventLabels { get; set; } = new();
        public string ScorePatternHint { get; set; } = "2:1";
        public bool AllowDraw { get; set; }
        public int MaxPeriods { get; set; }
    }

    public class SportMatchStateDto
    {
        public string MatchType { get; set; } = null!;
        public long MatchId { get; set; }
        public long IdEvent { get; set; }
        public string Sport { get; set; } = null!;
        public string Status { get; set; } = "Upcoming";
        public string FirstParticipant { get; set; } = null!;
        public string SecondParticipant { get; set; } = null!;
        public string? Score { get; set; }
        public string? Winner { get; set; }
        public int CurrentPeriod { get; set; }
        public string? Time { get; set; }
        public List<SportLiveEventDto> Timeline { get; set; } = new();
        public Dictionary<string, string> Stats { get; set; } = new();
        public SportRuleDto Rules { get; set; } = new();
        public bool CanEdit { get; set; }
        public List<string> ValidationMessages { get; set; } = new();
    }

    public class SportLiveEventDto
    {
        public long Id { get; set; }
        public string Type { get; set; } = null!;
        public string Participant { get; set; } = null!;
        public string? Player { get; set; }
        public int? Minute { get; set; }
        public int? Period { get; set; }
        public string? Value { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class SportLiveEventSubmitDto
    {
        public string MatchType { get; set; } = null!;
        public long MatchId { get; set; }
        public string? Login { get; set; }
        public string? Role { get; set; }
        public string Type { get; set; } = null!;
        public string Participant { get; set; } = null!;
        public string? Player { get; set; }
        public int? Minute { get; set; }
        public int? Period { get; set; }
        public string? Value { get; set; }
        public string? Notes { get; set; }
    }

    public class SportFinalResultSubmitDto
    {
        public string MatchType { get; set; } = null!;
        public long MatchId { get; set; }
        public string? Login { get; set; }
        public string? Role { get; set; }
        public string Score { get; set; } = null!;
        public string? Winner { get; set; }
        public Dictionary<string, string> Stats { get; set; } = new();
        public string? Notes { get; set; }
        public bool FinishMatch { get; set; } = true;
    }

    public class SportScoreValidationResultDto
    {
        public bool IsValid { get; set; }
        public string? Winner { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
