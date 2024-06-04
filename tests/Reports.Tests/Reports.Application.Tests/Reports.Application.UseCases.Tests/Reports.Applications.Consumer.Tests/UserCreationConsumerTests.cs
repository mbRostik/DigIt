﻿using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reports.Application.UseCases.Commands;
using Reports.Application.UseCases.Consumers;
using MassTransit;
using MessageBus.Messages.Commands.IdentityServerService;
using MediatR;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using MessageBus.Messages.Events.IdentityServerService;
namespace Reports.Application.UseCases.Tests.Reports.Applications.Consumer.Tests
{
    public class UserCreationConsumerTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ConsumeContext<IUserCreate_Send_To_ReportWebApi>> _contextMock = new Mock<ConsumeContext<IUserCreate_Send_To_ReportWebApi>>();
        private readonly UserCreation_Consumer _consumer;

        public UserCreationConsumerTests()
        {
            _consumer = new UserCreation_Consumer(_mediatorMock.Object);
        }

        [Fact]
        public async Task Consume_ShouldCreateUser()
        {
            // Arrange
            var userId = "user123";
            var userCreationDTO = new UserCreationDTO
            {
                UserId = userId,
                Status = UserCreationStatuses.PostWebApi_Created
            };
            var messageMock = new Mock<IUserCreate_Send_To_ReportWebApi>();
            messageMock.SetupGet(x => x.Data).Returns(userCreationDTO);
            messageMock.SetupGet(x => x.CorrelationId).Returns(Guid.NewGuid());

            _contextMock.SetupGet(x => x.Message).Returns(messageMock.Object);

            // Act
            await _consumer.Consume(_contextMock.Object);

            // Assert
            _mediatorMock.Verify(x => x.Send(It.Is<CreateUserCommand>(cmd => cmd.model.Id == userId), It.IsAny<System.Threading.CancellationToken>()));
        }

        [Fact]
        public async Task Consume_ShouldHandleExceptionAndNotPublishEvent()
        {
            // Arrange
            var userId = "user456";
            var userCreationDTO = new UserCreationDTO
            {
                UserId = userId,
                Status = UserCreationStatuses.UserWebApi_Created
            };
            var messageMock = new Mock<IUserCreate_Send_To_ReportWebApi>();
            messageMock.SetupGet(x => x.Data).Returns(userCreationDTO);
            messageMock.SetupGet(x => x.CorrelationId).Returns(Guid.NewGuid());

            _contextMock.SetupGet(x => x.Message).Returns(messageMock.Object);
            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<System.Threading.CancellationToken>()))
                         .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act
            Func<Task> act = async () => await _consumer.Consume(_contextMock.Object);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(act);
            _contextMock.Verify(x => x.Publish(It.IsAny<IUserCreate_SendEvent_From_ReportWebApi>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
        }
    }
}
