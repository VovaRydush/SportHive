using System.Text.Json;
using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.SaveServices.Interfaces;


namespace SportHive.Implementations
{

    public class SaveDataDb : ISaveDataDb
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<KafkaWorker> _logger;
        public SaveDataDb(ILogger<KafkaWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

        }
        public async Task SaveDataUser(string jsonObj, string topic)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            try
            {
                switch (topic)
                {
                    case "user_regist":
                        var user = JsonSerializer.Deserialize<User>(jsonObj);
                        dbContext.Users.Add(user);
                        break;

                    case "user-athlete":
                        var athlete = JsonSerializer.Deserialize<Athlete>(jsonObj);
                        dbContext.Athletes.Add(athlete);
                        break;

                    case "user-trainer":
                        var trainer = JsonSerializer.Deserialize<Trainer>(jsonObj);
                        dbContext.Trainers.Add(trainer);
                        break;

                    case "user-judge":
                        var judge = JsonSerializer.Deserialize<Judge>(jsonObj);
                        dbContext.Judges.Add(judge);
                        break;

                    case "user-photo":
                        var userphoto = JsonSerializer.Deserialize<UserPhoto>(jsonObj);
                        dbContext.UserPhotos.Add(userphoto);
                        break;

                    case "user-organization":
                        var organization = JsonSerializer.Deserialize<Organization>(jsonObj);
                        dbContext.Organizations.Add(organization);
                        break;

                    default:
                        _logger.LogWarning($"Unknown topic: {topic}");
                        return;
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
            }
        }
    }
}