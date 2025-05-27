using DB.SportHive.Domain;
using DB.SportHive.Persistence;
namespace SportHive.Services.Interfaces
{
    public interface ICompliteMatch
    {
        Task CompliteArchery(CompliteArcheryDto data);
        Task CompliteChess(CompliteChessDto chessDto);
        Task CompliteCycling(CompliteCyclingDto cyclingDto); // подумати що тут робити бо тут лише треба заповнити тип
        Task CompliteRowing(CompliteRowingDto rowingDto);
        Task CompliteWeightlifting(List<CompliteWeightliftingDto> weightliftingDto);
        Task CompliteExtremeMatch(AppDbContext _appDbContext,CompliteMatchInfo data);
        Task CompliteIndividualMatch(AppDbContext _appDbContext,CompliteMatchInfo data);
        Task CompliteTeamMatch(AppDbContext _appDbContext,CompliteMatchInfo data);
        
    }
}