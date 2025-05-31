using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace DB.SportHive.Domain
{
    public class AthleteProfile
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string FullName { get; set; } = null!;
        public string SportType { get; set; } = null!; // Наприклад, "Футбол", "Баскетбол"
        public string? Position { get; set; } // Якщо є

        public List<TournamentResult> TournamentHistory { get; set; } = new();
        public object SportStats { get; set; } = null!; // Тут буде конкретний клас по виду спорту
    }
}