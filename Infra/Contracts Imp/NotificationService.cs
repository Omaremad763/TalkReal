using Application.Contracts.IService;

using Domain.Events;

using Infra.Presistence;

using Microsoft.AspNetCore.SignalR;

namespace Infra.Contracts_Imp;
public class NotificationService(IHubContext<PresenceHub> hubContext) : INotificationService
{
    public async Task SendMessageNotificationAsync(MessageCreatedEvent notification, CancellationToken ct)
    {
        await hubContext.Clients.User(notification.ReceiverId)
            .SendAsync("ReceiveMessage", new
            {
                notification.MessageId,
                notification.SenderId,
                notification.Content,
                notification.CreatedAt
            }, ct);
    }
}