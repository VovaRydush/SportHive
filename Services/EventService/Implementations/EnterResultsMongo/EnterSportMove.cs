using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Exceptions;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class EnterSportMove : IEnterSportMove
    {
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        private readonly DisciplineFactory _disciplineFactory;
        public EnterSportMove(IMongoDbService mongoDbService, DisciplineFactory disciplineFactory)
        {
            _disciplineFactory = disciplineFactory;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
        }
        public async Task SetAttackMoves(TwoPlayerMoveDto move)
        {
            if (EnumWork.TryParseStyleFromText<TypeMoves>(move.typeMove, out int typeMoves))
            {
                var obj = _disciplineFactory.CreateTeamRecord(new TeamInfo { NameDesipline = move.NameSport });
                var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.idMatch, move.idMatch),
                    Builders<MatchEvents>.Filter.Eq("_t", "TeamDesiplines")
                );
                var update = Builders<MatchEvents>.Update.Combine(
                    Builders<MatchEvents>.Update.Push(nameof(TeamDesiplines.attacksMoves), new AttacksMoves
                    {
                        IdMatch = move.idMatch,
                        FullNamePlayer = move.FullNamePlayer1,
                        move = (TypeMoves)typeMoves,
                        time = move.timeMove,
                        realization =  move.realization
                    })
                );

                await _matchEvents.UpdateOneAsync(filter, update);
            }
            else throw new NotFoundException("Not found type move");
            await Task.CompletedTask;
        }

        public Task SetFoul(PlayerMovesDto move)
        {
            throw new NotImplementedException();
        }

        public Task SetPlayMoves(TwoPlayerMoveDto move)
        {
            throw new NotImplementedException();
        }

        public Task SetTimeOut(TeamMovesDto teamMoves)
        {
            throw new NotImplementedException();
        }

        public Task SetTouchdown(TwoPlayerMoveDto move)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoPlayersMove(TwoPlayerMoveDto playerMoveDto)
        {
            throw new NotImplementedException();
        }
    }
}