using System.ComponentModel;
using DB.SportHive.Domain;
using SportHive.Services.Interfaces;
using SportHive.Implementations;
namespace DB.SportHive.MongoDb
{
    public class Box : MatchEvents
    {
        public Box(){}
        public Box(TwoPlayerInfo info)
        {
            idMatch = info.idMatch;
            tour = info.tour;
            FullNamePlayer1 = info.FullNamePlayer1;
            FullNamePlayer2 = info.FullNamePlayer2;
        }
        public int tour { get; set; }
        public WinStruggleResult boxWinner { get; set; } = null!;
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 { get; set; } = null!;
        public List<RoundPoints> points {get;set;} = new();
        public List<PlayerFouls> fouls { get; set; } = new();
    }

    internal interface ICompetitionSystem
    {
    }
}
