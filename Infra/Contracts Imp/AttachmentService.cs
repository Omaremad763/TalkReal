using Application.Contracts;
using Application.Contracts.IService;
using Application.DTOS;

using Infra.Presistence;

using Microsoft.AspNetCore.SignalR;

namespace Infra.Contracts_Imp;
public class AttachmentService(IUnitofWork unitOfWork,
    IHubContext<PresenceHub> hubContext
    )
   : IAttachmentService
{
    public async Task<bool> ProcessMediaWebhookAsync(CloudinaryHookDto DTO)
    {
        var message = await unitOfWork.MessageRepo.GetMessageByPublicIdAsync(DTO.PublicId);

        if (message == null) return false;

        message.Attachment = message.Attachment with
        {
            Url = DTO.SecureUrl,
            IsProcessed = true
        };

        var result = await unitOfWork.CommitAsync() > 0;

        if (result)
        {
            await hubContext.Clients.Users(message.SenderId, message.ReceiverId)
           .SendAsync("FileProcessed", new { messageId = message.Id, url = DTO.SecureUrl });
        }

        return result;
    }
}

