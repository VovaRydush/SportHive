using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    public class OrganizationProfile
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Type { get; set; } = null!; // "Федерація", "Спортивний клуб", "Навчальний заклад", "Організатор"
        public string Status { get; set; } = null!; // "Активна", "Очікує підтвердження", "Заблокована"

        public string ContactPhone { get; set; } = null!;
        public string Website { get; set; } = null!;

        public List<string> Teams { get; set; } = new();         // ID команд
        public List<string> Judges { get; set; } = new();        // ID суддів
        public List<string> EventsCreated { get; set; } = new(); // ID заходів

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public bool IsVerified { get; set; } = false;
        public string VerificationDocumentUrl { get; set; } = null!;
    }
}
