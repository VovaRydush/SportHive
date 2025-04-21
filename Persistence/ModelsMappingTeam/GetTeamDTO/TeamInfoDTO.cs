namespace DB.SportHive.Domain
{
    public class TeamInfoDto
    {
        public string NameTeam{get;set;} = null!;
        public string TrainerFirstName{get;set;} = null!;
        public string TrainerLastName{get;set;} = null!;
        public string? TrainerPhotp{get;set;}= null!;
        public string TrainerLogin{get;set;} = null!;
        public string? PhotoTeam{get;set;} = null!;
        public string TypeSport{get;set;} = null!;
    }
}