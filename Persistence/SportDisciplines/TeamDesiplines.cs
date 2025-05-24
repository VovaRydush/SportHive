using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class TeamDesiplines : MatchEvents
    {
        public TeamDesiplines(TeamInfo info)
        {
            Tour = info.Tour;
            NameDesipline = info.NameDesipline;
            idMatch = info.idMatch;
            Group = info.Group ?? -1;
        }
        public int Tour { get; set; }
        public int Group { get; set; }
        public string NameDesipline { get; set; } = null!; // футбол, баскетбол, волейбол, пляжний волейбол, амириканський футбол, хокей,Регбі,Бейсбол
        public TeamScore firstTeamScore { get; set; } = null!;
        public TeamScore secondTeamScore { get; set; } = null!;
        public List<PlayerFouls> fouls { get; set; } = null!;
        public List<MoveTwoPlayer> twoPlayersMoves { get; set; } = null!;
        public List<PlayMoves> playMoves { get; set; } = null!;
        public List<AttacksMoves> attacksMoves { get; set; } = null!;
        public List<TimeOut>? timeOuts { get; set; } = null!;
        public List<Touchdown>? touchdowns { get; set; } = null!;
    }
}