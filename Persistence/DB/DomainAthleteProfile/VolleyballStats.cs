namespace DB.SportHive.Domain
{
    public class VolleyballStats : SportStats
    {
        public int Matches { get; set; }
        public int Blocks { get; set; }
        public int Errors { get; set; }
        public int Win { get; set; }
        public int Losses { get; set; }
    }
}
