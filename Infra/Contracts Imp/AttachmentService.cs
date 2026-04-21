using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.Contracts.IService;
using Application.CQRS;
using Application.DTOS;

using CloudinaryDotNet.Actions;

using Domain.Entities;
using Domain.Enum;
using Domain.Value_Object;

using Infra.Presistence;

using MediatR;

using Microsoft.AspNetCore.SignalR;

namespace Infra.Contracts_Imp;
public class AttachmentService(IUnitofWork unitOfWork, 
    IHubContext<PresenceHub> hubContext
    ) 
   : IAttachmentService
{
    public async Task<bool> ProcessMediaWebhookAsync(CloudinaryHookDTO DTO)
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

