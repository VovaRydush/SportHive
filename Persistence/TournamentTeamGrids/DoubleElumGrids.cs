
namespace DB.SportHive.Domain
{
    public class TopGrig : TeamIndivGrid
    {
        public string ParentMatch1 { get; set; } = null!;
        public string ParentMatch2 { get; set; } = null!;
        public string NameWinner { get; set; } = null!;
    }
    public class BottomGrid : TeamIndivGrid
    {
        public string ParentMatch1 { get; set; } = null!;
        public string ParentMatch2 { get; set; } = null!;
        public string NameWinner { get; set; } = null!;
    }
}
