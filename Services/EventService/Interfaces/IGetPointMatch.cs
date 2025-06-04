namespace SportHive.Services.Interfaces
{
    public interface IGetPointMatch
    {
        Task<float> GetPoints(string sport, string result);
    }
}