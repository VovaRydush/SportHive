

namespace DB.SportHive.Domain
{
    public class TeamModelDto
    {
        public string NameTeam { get; set; } = null!;
        public string LoginTrainer { get; set; } = null!;
        public string TypeSport { get; set; } = null!;
        public List<TeamAthleteDto> athlets{get;set;} = null!;
    }
   
    public class TeamAthleteDto{
        public string NameTeam { get; set; } = null!;
        public string  loginAthlets { get; set; } =  null!;
        public string AthleteStatus { get; set; }=null!;
    }
    
}