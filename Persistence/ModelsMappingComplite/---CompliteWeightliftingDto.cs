using DB.SportHive.MongoDb;

namespace DB.SportHive.Domain
{
    public class CompliteWeightliftingDto
    {
        public string FullNamePlayer { get; set; } = null!;
        public WeightDesipline desipline { get; set; }
        public string WeightCategory { get; set; } = null!;
        public float Weight { get; set; }
        public CompliteMatchInfo MatchInfo { get; set; } = null!;
    }
}