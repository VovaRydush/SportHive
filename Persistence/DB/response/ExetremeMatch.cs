namespace DB.SportHive.Domain
{
    public class ExetremeMatchPlayer
    {
        List<ExetremeMatchRes> matchRes { get; set; } = null!;
    }
    public class ExetremeMatchRes
    {
        public string NameEvent { get; set; } = null!;
        public string DataEvent { get; set; } = null!;
        public string Place { get; set; } = null!;
        public string Time { get; set; } = null!;
    }
}