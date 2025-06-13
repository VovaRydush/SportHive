using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("CheckersChessStats", RootClass = false)]
    public class CheckersChessStats : SportStats
    {
        public CheckersChessStats() { }
        public int Matches { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public List<string> TournamentMedals { get; set; } = new();
        public int EloRating { get; set; }
        public int FastestWinMoves { get; set; }         // Найменша кількість ходів до перемоги
        public double AverageMoveTimeSeconds { get; set; }
    }
}