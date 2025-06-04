using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class GetPointMatch : IGetPointMatch
    {
        public async Task<float> GetPoints(string sport, string result)
        {
            var rules = new Dictionary<string, Dictionary<string, float>>
            {
                ["Football"] = new() { ["win"] = 3, ["draw"] = 1, ["loss"] = 0 },
                ["Basketball"] = new() { ["win"] = 2, ["draw"] = 1, ["loss"] = 0 },
                ["Chess"] = new() { ["win"] = 1, ["draw"] = 0.5f, ["loss"] = 0 },
                ["Beach Volleyball"] = new() { ["win"] = 2, ["draw"] = 0, ["loss"] = 0 },
                ["Hockey"] = new() { ["win"] = 2, ["draw"] = 1, ["loss"] = 0 },
                ["American football"] = new() { ["win"] = 2, ["draw"] = 0, ["loss"] = 0 },
                ["Checkers"] = new() { ["win"] = 1, ["draw"] = 0.5f, ["loss"] = 0 },
                ["Volleyball"] = new() { ["win"] = 2, ["draw"] = 0, ["loss"] = 0 }
            };

            if (rules.ContainsKey(sport) && rules[sport].ContainsKey(result))
            {
                return rules[sport][result];
            }
            else return -1;
        }
    }
}