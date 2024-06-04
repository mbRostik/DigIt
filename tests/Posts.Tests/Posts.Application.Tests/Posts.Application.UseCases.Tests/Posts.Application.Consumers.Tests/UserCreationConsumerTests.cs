﻿using MassTransit;
using MediatR;
using Moq;
using Serilog;
using System;
using System.Threading.Tasks;
using Xunit;
using Posts.Application.UseCases.Consumers;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using MessageBus.Messages.Commands.IdentityServerService;
using Posts.Application.UseCases.Commands;
public class UserCreationConsumerTests
{
    private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
    private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
    private readonly UserCreation_Consumer _consumer;

    public UserCreationConsumerTests()
    {
        _consumer = new UserCreation_Consumer(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldHandleUserCreation()
    {
        // Arrange
        var userId = "dfqfq";
        var userEmail = "test@example.com";
        var userName = "TestUser";

        var userCreationDTO = new UserCreationDTO
        {
            UserId = userId,
            UserEmail = userEmail,
            UserName = userName,
            Status = UserCreationStatuses.PostWebApi_Created 
        };

        var mockMessage = new Mock<ConsumeContext<IUserCreate_Send_To_PostWebApi>>(); 
        var userCreateSendToPostWebApiMock = new UserCreateSendToPostWebApiMock(Guid.NewGuid(), userCreationDTO); 

        mockMessage.SetupGet(x => x.Message).Returns(userCreateSendToPostWebApiMock);

        // Act
        await _consumer.Consume(mockMessage.Object);

        // Assert
        _mediatorMock.Verify(x => x.Send(It.Is<CreateUserCommand>(cmd =>
            cmd.model.Id == userId),
            It.IsAny<CancellationToken>()), Times.Once);

    }
}

public class UserCreateSendToPostWebApiMock : IUserCreate_Send_To_PostWebApi
{
    public Guid CorrelationId { get; }
    public UserCreationDTO Data { get; }

    public UserCreateSendToPostWebApiMock(Guid correlationId, UserCreationDTO data)
    {
        CorrelationId = correlationId;
        Data = data;
    }
}
