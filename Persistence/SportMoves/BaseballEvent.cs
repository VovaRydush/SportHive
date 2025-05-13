using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class BaseballEvent
    {
        public long IdMatch { get;}           // Ідентифікатор матчу
        public string FullNamePlayer { get; set; } = null!;        // Ідентифікатор гравця
        public string loginPlayer{get;set;} = null!;
        public DateTime EventTime { get; set; }     // Час події
        public EventType EventType { get; set; }    // Тип події
        public int? BaseReached { get; set; }       // Номер бази, яку досяг гравець (якщо застосовно)
    }
    public enum EventType
    {
        [Description("Strike")]
        Strike,            // Страйк
        [Description("Ball")]
        Ball,              // Бал
        [Description("Hit")]
        Hit,               // Хіт
        [Description("Out")]
        Out,               // Ауто
        [Description("Walk")]
        Walk,              // Прогулянка (коли дають безкоштовно пройти до першої бази)
        [Description("Error")]
        Error,             // Помилка
        [Description("HomeRun")]
        HomeRun,           // Хоум-ран
        [Description("StolenBase")]
        StolenBase,        // Викрадена база
        [Description("DoublePlay")]
        DoublePlay,        // Подвійний гейм
        [Description("TriplePlay")]
        TriplePlay,        // Триразовий гейм
        [Description("Pitch")]
        Pitch,             // Кидок пітчера
        [Description("PitchHit")]
        PitchHit,          // Удар по м'ячу
        [Description("Strikeout")]
        Strikeout          // Страйк-аут (коли три страйка)
    }
}
