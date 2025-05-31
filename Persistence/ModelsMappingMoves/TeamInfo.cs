namespace DB.SportHive.Domain
{
    public class TeamInfo
    {
        public long idMatch { get; set; }
        public long idEvent { get; set; }
        public string loginJudge { get; set; } = null!;
        public string NameDesipline { get; set; } = null!;
        public int Tour { get; set; } 
        public int? Group{ get; set; }
    }
}