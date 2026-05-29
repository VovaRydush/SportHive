namespace DB.SportHive.Domain
{
    public class TeamInfoDto
    {
        public string NameTeam { get; set; } = null!;
        public string TrainerFirstName { get; set; } = null!;
        public string TrainerLastName { get; set; } = null!;
        public string? TrainerPhotp { get; set; } = null!;
        public string TrainerLogin { get; set; } = null!;
        public string? PhotoTeam { get; set; } = null!;
        public string TypeSport { get; set; } = null!;
        public List<AthleteTeamDto> Athletes { get; set; } = new();
        public List<OrganizationTeamInfoDto> Organizations { get; set; } = new();
        public int AthletesCount => Athletes.Count;
    }

    public class OrganizationTeamInfoDto
    {
        public string LoginOrganization { get; set; } = null!;
        public string NameOrganization { get; set; } = null!;
        public string? Country { get; set; }
        public string? TypeOrganozation { get; set; }
    }
}
