using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.Contracts.IService;
using Application.CQRS;
using Application.DTOS;

using Domain.Events;

using FluentAssertions;

using FluentValidation.TestHelper;

using Moq;

using Xunit;

namespace TalkRealTestProject;



public class ChatApplicationTests
{
    private readonly Mock<ITalkRealServices> _serviceMock;
    private readonly ChatApplicationHandler _handler;
    private readonly SendMessageValidator _validator;

    public ChatApplicationTests()
    {
        _serviceMock = new Mock<ITalkRealServices>();
        _handler = new ChatApplicationHandler(_serviceMock.Object);
        _validator = new SendMessageValidator();
    }


    [Fact]
    public async Task Handle_SendMessageCommand_ShouldReturnTrue_WhenServiceSucceeds()
    {
        var command = new SendMessageCommand(new MessageDto { Content = "Hello", ReceiverId = "user-123" });
        _serviceMock.Setup(x => x.MessageService.SendMessageAsync(command.Data))
                    .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(x => x.MessageService.SendMessageAsync(command.Data), Times.Once);
    }

    [Fact]
    public async Task Handle_MessageCreatedEvent_ShouldInvokeNotificationService()
    {
        var notification = new MessageCreatedEvent(Guid.NewGuid(), "receiver-id", "Hello world","test","test url");
        var notificationServiceMock = new Mock<INotificationService>();
        _serviceMock.Setup(x => x.NotificationService).Returns(notificationServiceMock.Object);
        await _handler.Handle(notification, CancellationToken.None);
        notificationServiceMock.Verify(x => x.SendMessageNotificationAsync(notification, It.IsAny<CancellationToken>()), Times.Once);
    }


    [Theory]
    [InlineData("", "user-123", "Data.Content")]         
    [InlineData("Valid Content", "", "Data.ReceiverId")]    
    public void SendMessageValidatorShouldHaveSpecificErrorWhenDataIsInvalid(string content, string receiverId, string expectedErrorLocation)
    {
        var command = new SendMessageCommand(new MessageDto { Content = content, ReceiverId = receiverId });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(expectedErrorLocation);
    }

    [Fact]
    public void SendMessageValidator_ShouldNotHaveError_WhenDataIsValid()
    {
        var command = new SendMessageCommand(new MessageDto { Content = "Valid Message", ReceiverId = "receiver-id" });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}

