using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class TeamDesiplines : MatchEvents
    {
        public TeamDesiplines(){}
        public TeamDesiplines(TeamInfo info)
        {
            Tour = info.Tour;
            idEvent = info.idEvent;
            NameDesipline = info.NameDesipline;
            idMatch = info.idMatch;
            Group = info.Group ?? -1;
        }
        public int Tour { get; set; }
        public int Group { get; set; }
        public string NameDesipline { get; set; } = null!;
        public TeamScore firstTeamScore { get; set; } = null!;
        public TeamScore secondTeamScore { get; set; } = null!;
        public List<PlayerFouls> fouls { get; set; }  = new();
        public List<MoveTwoPlayer> twoPlayersMoves { get; set; }  = new();
        public List<PlayMoves> playMoves { get; set; } = new();
        public List<AttacksMoves> attacksMoves { get; set; } = new();
        public List<TimeOut>? timeOuts { get; set; }  = new();
        public List<Touchdown>? touchdowns { get; set; } = new();
    }
}