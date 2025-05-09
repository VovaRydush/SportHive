using System.ComponentModel;
using MongoDB.Bson;

namespace DB.SportHive.MongoDb
{
    public class CyclingRace
    {
        public long IdMatch { get; set; }
        public string AthleteName { get; set; } = null!;
        public string RaceType { get; set; } = null!;
        public float DistanceKm { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int Position { get; set; }
        public float AvgSpeed { get; set; }
        public float? MaxSpeed { get; set; }
        public List<CyclingFoul> Fouls { get; set; } = new();
        public bool DidNotFinish { get; set; }
    }

    public enum CyclingFoul
    {
        [Description("UnsportingBehavior")]
        UnsportingBehavior,
        [Description("DraftingViolation")]
        DraftingViolation,
        [Description("LineChangeInSprint")]
        LineChangeInSprint,
        [Description("Littering")]
        Littering,
        [Description("IllegalAssistance")]
        IllegalAssistance
    }
}
