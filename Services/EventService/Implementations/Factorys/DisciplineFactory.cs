using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class DisciplineFactory
    {
        private readonly IServiceProvider _provider;

        public DisciplineFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public MatchEvents CreateIndividual(TwoPlayerInfo info)
        {
            return info.NameDesipline switch
            {
                "Box" => new Box(info),
                "Checkers" => new CheckersGame(info),
                "Chess" => new ChessMatch(info),
                "CortMatch" => new CortMatches(info),
                "Struggle" => new Struggle(info),
                _ => throw new NotImplementedException($"Unknown system: {info.NameDesipline}")
            };
        }
        public MatchEvents CreateTeamRecord(TeamInfo info)
        {
            var teamDisciplines = new HashSet<string>
            {
                "Football",
                "Basketball",
                "Volleyball",
                "BeachVolleyball",
                "AmericanFootball", 
                "Hockey",
                "Rugby",
                "Baseball"
            };

            if (teamDisciplines.Contains(info.NameDesipline))
            {
                return new TeamDesiplines(info);
            }

            throw new NotImplementedException($"Unknown system: {info.NameDesipline}");
        }
        public MatchEvents CreateExteme(ExtremeIndividualInfo info)
        {
            return info.NameDesipline switch
            {
                "AthleticsMatch" => new AthleticsMatch(info),
                "Cycling" => new Cycling(info),
                "DistanceRunning" => new DistanceRunningMatch(info),
                "Rowing" => new Rowing(info),
                "Swimming"=>new Swimming(info),
                "WeightliftingMatch" => new WeightliftingMatch(info),
                "Archery" => new Archery(info),
                _ => throw new NotImplementedException($"Unknown system: {info.NameDesipline}")
            };
        }
    }

}
