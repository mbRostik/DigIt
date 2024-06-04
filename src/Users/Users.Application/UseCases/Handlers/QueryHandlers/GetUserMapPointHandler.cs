using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;
using Users.Application.UseCases.Queries;
using Users.Infrastructure.Data;

namespace Users.Application.UseCases.Handlers.QueryHandlers
{
    public class GetUserMapPointHandler : IRequestHandler<GetUserMapPointQuery, List<GiveUserMapPointsDTO>>
    {
        public readonly Serilog.ILogger logger;

        private readonly UserDbContext dbContext;

        public GetUserMapPointHandler(UserDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<List<GiveUserMapPointsDTO>> Handle(GetUserMapPointQuery request, CancellationToken cancellationToken)
        {
            try
            {
                logger.Information("Handling GetUserMapPointQuery");

                var pointsIds = await dbContext.MapPointWithUsers.Where(x => x.UserId == request.id).Select(x => x.PointId).ToListAsync();

                var points = await dbContext.MapPoints.Where(mappoint => pointsIds.Contains(mappoint.Id)).ToListAsync();

                List<GiveUserMapPointsDTO> result= new List <GiveUserMapPointsDTO>();

                foreach (var point in points)
                {
                    GiveUserMapPointsDTO temp = new GiveUserMapPointsDTO
                    {
                        Title = point.Title,
                        Description = point.Description,
                        Position = point.Position,
                        Id = point.Id,
                    };
                    result.Add(temp);
                }

                if (result.Any())
                {
                    logger.Information("Successfully retrieved {Count} points.", result.Count);
                }
                else
                {
                    logger.Information("No points found.");
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while handling GetUserMapPointQuery");
                throw;
            }
        }


    }
}