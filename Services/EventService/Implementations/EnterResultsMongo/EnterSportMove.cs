using System.Runtime.Remoting;
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
                var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.idMatch, move.idMatch)
                );
                var update = Builders<MatchEvents>.Update.Combine(
                    Builders<MatchEvents>.Update.Push("attacksMoves", new AttacksMoves
                    {
                        IdMatch = move.idMatch,
                        FullNamePlayer = move.FullNamePlayer,
                        move = (TypeMoves)typeMoves,
                        time = move.timeMove,
                        realization = move.realization
                    })
                );

                await _matchEvents.UpdateOneAsync(filter, update);
            }
            else throw new NotFoundException("Not found type move");
            await Task.CompletedTask;
        }

        public async Task SetFoul(PlayerMovesDto move)
        {
            if (EnumWork.TryParseStyleFromText<Foul>(move.typeMove, out int typeMoves))
            {
                var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.idMatch, move.idMatch)
                );
                var update = Builders<MatchEvents>.Update.Combine(
                    Builders<MatchEvents>.Update.Push("fouls", new PlayerFouls
                    {
                        FullNamePlayer = move.FullNamePlayer,
                        loginPlayer = move.login,
                        timeFoul = move.timeMove,
                        foul = (Foul)typeMoves,
                    })
                );

                await _matchEvents.UpdateOneAsync(filter, update);
            }
            else throw new NotFoundException("Not found type move");
        }

        public async Task SetPlayMoves(TwoPlayerMoveDto move)
        {
            if (EnumWork.TryParseStyleFromText<TypeMove>(move.typeMove, out int typeMoves))
            {
                var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.idMatch, move.idMatch)
                );
                var update = Builders<MatchEvents>.Update.Combine(
                    Builders<MatchEvents>.Update.Push("playMoves", new PlayMoves
                    {
                        IdMatch = move.idMatch,
                        FullNamePlayer = move.FullNamePlayer,
                        typeMove = (TypeMove)typeMoves,
                        timeMove = move.timeMove
                    })
                );
                await _matchEvents.UpdateOneAsync(filter, update);
            }
            else throw new NotFoundException("Not found type move");
        }

        public async Task SetStruggleFouls(TwoPlayerMoveDto move)
        {
             if (EnumWork.TryParseStyleFromText<Foul>(move.typeMove, out int typeMoves))
            {
                var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.idMatch, move.idMatch)
                );
                var update = Builders<MatchEvents>.Update.Combine(
                    Builders<MatchEvents>.Update.Push($"player{move.whoPlayer}Fouls", new PlayerFouls
                    {
                        FullNamePlayer = move.FullNamePlayer,
                        timeFoul = move.timeMove,
                        foul = (Foul)typeMoves,
                    })
                );

                await _matchEvents.UpdateOneAsync(filter, update);
            }
            else throw new NotFoundException("Not found type move");
        }

        public async Task SetTimeOut(TeamMovesDto teamMoves)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq(x => x.idMatch, teamMoves.idMatch)
            );
            var update = Builders<MatchEvents>.Update.Combine(
                Builders<MatchEvents>.Update.Push("timeOuts", new TimeOut
                {
                    IdMatch = teamMoves.idMatch,
                    NameTeam = teamMoves.NameTeam,
                    TimeStartTimeOut = teamMoves.TimeStartTimeOut,
                    TimeEndTimeOut = teamMoves.TimeEndTimeOut
                })
            );
            await _matchEvents.UpdateOneAsync(filter, update);
        }

        public async Task SetTouchdown(TwoPlayerMoveDto move)
        {
            if (EnumWork.TryParseStyleFromText<TypeTouchdown>(move.typeMove, out int typeMoves))
            {
                var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.idMatch, move.idMatch)
                );
                var update = Builders<MatchEvents>.Update.Combine(
                    Builders<MatchEvents>.Update.Push("touchdowns", new Touchdown
                    {
                        IdMatch = move.idMatch,
                        FullNamePlayer = move.FullNamePlayer,
                        typeTouchdown = (TypeTouchdown)typeMoves,
                        time = move.timeMove,
                        realization = move.realization,
                        yards = move.yards
                    })
                );
                await _matchEvents.UpdateOneAsync(filter, update);
            }
            else throw new NotFoundException("Not found type move");
        }

        public async Task SetTwoPlayersMove(TwoPlayerMoveDto playerMoveDto)
        {
            if (EnumWork.TryParseStyleFromText<TypeMovePlayer>(playerMoveDto.typeMove, out int typeMoves))
            {
                var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.idMatch, playerMoveDto.idMatch)
                );
                var update = Builders<MatchEvents>.Update.Combine(
                    Builders<MatchEvents>.Update.Push("twoPlayersMoves", new MoveTwoPlayer
                    {
                        IdMatch = playerMoveDto.idMatch,
                        FullNamePlayer = playerMoveDto.FullNamePlayer,
                        loginPlayer = playerMoveDto.loginPlayer,
                        typeMove = (TypeMovePlayer)typeMoves,
                        time = playerMoveDto.timeMove,
                    })
                );
                await _matchEvents.UpdateOneAsync(filter, update);
            }
            else throw new NotFoundException("Not found type move");
        }
    }
}