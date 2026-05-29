using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace DB.SportHive.Domain
{
    public class TeamModelDto
    {
        public string NameTeam { get; set; } = null!;
        public string LoginTrainer { get; set; } = null!;
        public string TypeSport { get; set; } = null!;
        public IFormFile? Photo { get; set; }
        public string? AthletsJson { get; set; }

        public List<TeamAthleteDto> Athlets
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AthletsJson))
                    return new List<TeamAthleteDto>();

                return JsonSerializer.Deserialize<List<TeamAthleteDto>>(
                    AthletsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<TeamAthleteDto>();
            }
        }
    }

    public class TeamAthleteDto
    {
        public string NameTeam { get; set; } = null!;
        public string LoginAthlets { get; set; } = null!;
        public string AthleteStatus { get; set; } = "Active";
    }
}