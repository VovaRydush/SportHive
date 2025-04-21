using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace DB.SportHive.Domain
{
    public class TeamModelDto
    {
        public string NameTeam { get; set; } = null!;
        public string LoginTrainer { get; set; } = null!;
        public string TypeSport { get; set; } = null!;
        public IFormFile Photo { get; set; } = null!;

        public string AthletsJson { get; set; } = null!;

        public List<TeamAthleteDto> Athlets =>
            string.IsNullOrEmpty(AthletsJson)
                ? new List<TeamAthleteDto>()
                : JsonSerializer.Deserialize<List<TeamAthleteDto>>(AthletsJson)!;
    }

    public class TeamAthleteDto
    {
        public string NameTeam { get; set; } = null!;
        public string LoginAthlets { get; set; } = null!;
        public string AthleteStatus { get; set; } = null!;
    }
}
