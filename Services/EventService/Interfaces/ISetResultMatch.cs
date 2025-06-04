namespace SportHive.Services.Interfaces
{
    public interface ISetResultMatch
    {
        Task SetResultMatches(string nameWinner, string resultWinner, long idMatch,string typeMatch);
    }
}