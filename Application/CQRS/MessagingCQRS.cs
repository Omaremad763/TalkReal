using Application.Contracts;
using Application.DTOS;

using Domain.Events;

using FluentValidation;

using MediatR;

namespace Application.CQRS;
public record SendMessageCommand(MessageDto Data) : IRequest<bool>;
public record GetChatHistoryQuery(ChatHistoryRequestDto ChatHistoryDTO) : IRequest<IQueryable<MessageDto>>;

public class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.Data.Content).NotEmpty().MaximumLength(1000).WithMessage("Message cannot be empty or too long.");
        RuleFor(x => x.Data.ReceiverId).NotEmpty().WithMessage("Receiver is required.");
    }
}
public class ChatApplicationHandler(ITalkRealServices service) :
    IRequestHandler<SendMessageCommand, bool>,
    IRequestHandler<GetChatHistoryQuery, IQueryable<MessageDto>>,
    INotificationHandler<MessageCreatedEvent>
{
    public async Task<bool> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        => await service.MessageService.SendMessageAsync(request.Data);

    public async Task<IQueryable<MessageDto>> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
        => await Task.FromResult(service.MessageService.GetChatHistoryQuery(request.ChatHistoryDTO));

    public async Task Handle(MessageCreatedEvent notification, CancellationToken cancellationToken)
=> await service.NotificationService.SendMessageNotificationAsync(notification, cancellationToken);
}

