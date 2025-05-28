using DB.SportHive.MongoDb;

namespace DB.SportHive.Domain
{
    public class BoxWinnerDto
    {
        public long idMatch { get; set; }
        public string FullNamePlayer { get; set; } = null!;
        public int? round { get; set; }
        public string loginWinner { get; set; } = null!;
        public Result win { get; set; }
    }
}