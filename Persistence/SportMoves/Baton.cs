using System.ComponentModel;
using MongoDB.Bson;

namespace DB.SportHive.MongoDb
{
    public class Baton
    {
        public long idMatch { get; set; }
        public string NameTeam { get; set; } = null!;
        public string FullNamePlayer { get; set; } = null!;
        public TimeSpan time { get; set; }
        public int Stage { get; set; }
        public TransferringBaton transferring{get;set;}

    }
    public enum TransferringBaton
    {
        [Description("BatonFell")]
        BatonFell,
        [Description("TransmissionOutside")]
        TransmissionOutside,
        [Description("Done")]
        Done
    }
}