using DB.SportHive.Domain;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class InitalSystemGrid : IInitalSystemGrid
    {
        private readonly IMongoCollection<TeamIndivGrid> _sytemGrid;
        public InitalSystemGrid(IMongoDbService mongoDbService)
        {
            _sytemGrid = mongoDbService.GetCollection<TeamIndivGrid>("TeamIndivGrid");
        }
        public async Task InitalSystemGrids(long IdEvent, string NameFirstEntity, string NameSecondEntity, Matchs matchs)
        {
            TeamIndivGrid entity = null!;

            switch (matchs.system)
            {
                case "PlayOff":
                case "Knockout":
                case "Olympic":
                    entity = new SingleEliminationGrid
                    {
                        NameFirstEntity = NameFirstEntity,
                        NameSecondEntity = NameSecondEntity,
                        idEvent = IdEvent,
                        tour = matchs.tour,
                        played = false
                    };
                    break;

                case "DoubleElimination":
                    entity = new TopGrig
                    {
                        NameFirstEntity = NameFirstEntity,
                        NameSecondEntity = NameSecondEntity,
                        idEvent = IdEvent,
                        tour = matchs.tour,
                        played = false
                    };
                    break;

                case "Group":
                    entity = new GroupGrids
                    {
                        NameFirstEntity = NameFirstEntity,
                        NameSecondEntity = NameSecondEntity,
                        idEvent = IdEvent,
                        tour = matchs.tour,
                        GroupNumber = matchs.Group ?? -1,
                        played = false
                    };
                    break;

                case "RoundRobin":
                    entity = new RoundRobitnGrids
                    {
                        NameFirstEntity = NameFirstEntity,
                        NameSecondEntity = NameSecondEntity,
                        idEvent = IdEvent,
                        tour = matchs.tour,
                        played = false
                    };
                    break;

                case "Swiss":
                    entity = new SwissSystemGrid
                    {
                        NameFirstEntity = NameFirstEntity,
                        NameSecondEntity = NameSecondEntity,
                        idEvent = IdEvent,
                        tour = matchs.tour,
                        played = false,
                        ratingElo1 = matchs.ratingElo1 ?? 0,
                        ratingElo2 = matchs.ratingElo2 ?? 0
                    };
                    break;
            }

            if (entity != null)
            {
                await _sytemGrid.InsertOneAsync(entity);
            }
        }

    }
}