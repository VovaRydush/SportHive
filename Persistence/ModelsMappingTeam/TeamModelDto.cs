

namespace DB.SportHive.Domain
{
    public class TeamModelDto
    {
        public string NameTeam { get; set; } = null!;
        public string LoginTrainer { get; set; } = null!;
        public string TypeSport { get; set; } = null!;
        public List<AthletsStatuses> athlets{get;set;} = null!;
    }

    public class AthletsStatuses
    {
       public string loginAthlets { get; set; } = null!;
       public string statusAthlets{get;set;}= null!;
    }
}