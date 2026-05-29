namespace DB.SportHive.Domain
{
    public class AthleteTeamDto
    {
        public string FirsName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? login { get; set; } = null!;
        public string? TypeSport { get; set; }
        public string? AthleteStatus { get; set; }
        public string? Photo { get; set; }
    }
}
