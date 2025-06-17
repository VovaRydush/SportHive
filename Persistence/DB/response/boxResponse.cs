using System.Security.Cryptography.X509Certificates;

namespace DB.SportHive.Domain
{
    public class BoxResponse
    {
        public int Matches { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Knockouts { get; set; }
        public int RoundsFought { get; set; }
        public double AverageScorePerRound { get; set; }
        public int WeightCategory { get; set; }
    }
}