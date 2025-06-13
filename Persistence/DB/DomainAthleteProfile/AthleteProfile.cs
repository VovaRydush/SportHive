using System.Security.Cryptography;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace DB.SportHive.Domain
{
    public class AthleteProfile
    {
        public AthleteProfile()
        {

        }
        public AthleteProfile(string fullName, string Login, string sportType)
        {
            FullName = fullName;
            login = Login;
            SportType = sportType;
        }
        [BsonId]
        public ObjectId Id { get; set; }

        public string FullName { get; set; } = null!;
        public string login { get; set; } = null!;
        public string SportType { get; set; } = null!; // Наприклад, "Футбол", "Баскетбол"
        public string Position { get; set; } // Якщо є
        public DateTime dateLastUpdate { get; set; }
        public List<TournamentResult> TournamentHistory { get; set; } = new();
        [BsonElement("SportStats")]
        public SportStats SportStats { get; set; } = null!; // Тут буде конкретний клас по виду спорту
    }
}