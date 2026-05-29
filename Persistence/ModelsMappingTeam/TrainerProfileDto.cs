namespace DB.SportHive.Domain
{
    public class TrainerProfileDto
    {
        public string Login { get; set; } = null!;
        public string FirsName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DataBirth { get; set; }
        public string? Photo { get; set; }
        public List<TrainerTeamDto> Teams { get; set; } = new();
        public List<TrainerOrganizationDto> Organizations { get; set; } = new();
        public TrainerStatsDto Stats { get; set; } = new();
    }

    public class TrainerTeamDto
    {
        public string NameTeam { get; set; } = null!;
        public string TypeSport { get; set; } = null!;
        public string? PhotoTeam { get; set; }
        public int AthletesCount { get; set; }
    }

    public class TrainerOrganizationDto
    {
        public string LoginOrganization { get; set; } = null!;
        public string NameOrganization { get; set; } = null!;
        public string TypeOrganozation { get; set; } = null!;
        public string Country { get; set; } = null!;
    }

    public class TrainerStatsDto
    {
        public int TeamsCount { get; set; }
        public int AthletesCount { get; set; }
        public int OrganizationsCount { get; set; }
        public int TotalMatches { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
    }
}
