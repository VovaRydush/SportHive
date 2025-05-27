namespace DB.SportHive.Domain
{
    public class PointsIntBoxStruggle
    {
        public long idMatch { get; set; }
        public string typeSport { get; set; } = null!;
        public string FullNamePlayer { get; set; } = null!;
        public int round { get; set; }
        public int countPoints { get; set; }
    } 
}