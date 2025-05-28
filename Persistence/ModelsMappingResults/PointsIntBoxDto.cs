namespace DB.SportHive.Domain
{
    public class PointsIntBoxStruggle
    {
        public long idMatch { get; set; }
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer { get; set; } = null!;
        public int round { get; set; }
        public int count { get; set; }
    } 
}