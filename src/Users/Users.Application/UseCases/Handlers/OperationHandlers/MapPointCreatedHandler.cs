using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.UseCases.Commands;
using Users.Domain.Entities;
using Users.Infrastructure.Data;

namespace Users.Application.UseCases.Handlers.OperationHandlers
{
    public class MapPointCreatedHandler : IRequestHandler<CreateMapPointCommand>
    {
    

        private readonly UserDbContext dbContext;
        public readonly Serilog.ILogger _logger;


        public MapPointCreatedHandler(UserDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(CreateMapPointCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Attempting to create a new mapPoint");

            try
            {
                _logger.Information("MapPoint: " + request.model.Title +" "+ request.model.Description+" !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                MapPoint mapPoint = new MapPoint
                {
                    Title = request.model.Title,
                    Description = request.model.Description,
                    Position = request.model.Position
                };
                var model = await dbContext.MapPoints.AddAsync(mapPoint);
                await dbContext.SaveChangesAsync(cancellationToken);

                MapPointWithUser mapPointWithUser = new MapPointWithUser
                {
                    UserId = request.userId,
                    PointId = model.Entity.Id
                };
                var model2 = await dbContext.MapPointWithUsers.AddAsync(mapPointWithUser);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.Information("MapPoint created successfully with ID {PostId}", model.Entity.Id);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while creating a new MapPoint");
            }
        }
    }
}