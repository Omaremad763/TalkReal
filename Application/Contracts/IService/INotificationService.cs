using Domain.Events;

namespace Application.Contracts.IService;

public interface INotificationService
{
    Task SendMessageNotificationAsync(MessageCreatedEvent notification, CancellationToken ct);
}