using System.ComponentModel;
using MongoDB.Bson;

namespace DB.SportHive.MongoDb
{
    public class PointBasketball
    {
        public long IdMatch{get;set;}
        public string FullNamePlayer{get;set;} = null!;
        public string timeGetPoint {get;set;} = null!;
        public PointType pointType {get;set;}
        public int valuePoint{get;set;}
    }

    public enum PointType
    {
        [Description("free_throw")]
        free_throw,
        [Description("two_point")]
        two_point,
        [Description("three_point")]
        three_point
    }
}