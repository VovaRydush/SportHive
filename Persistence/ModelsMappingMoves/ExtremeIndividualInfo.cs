using StackExchange.Redis;

namespace DB.SportHive.Domain
{
    public class ExtremeIndividualInfo
    {
        public long idMatch;
        public long idEvent;
        public string loginJudge { get; set; } = null!;
        public int tour;
        public string NameDesipline { get; set; } = null!;
        public List<EntityInfo> entitysName { get; set; } = null!;
    }
    public class EntityInfo
    {
        public string EntityName { get; set; } = null!;
        public string? loginPlayer { get; set; } = null!;
    }
}