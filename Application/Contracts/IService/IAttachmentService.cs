using Application.DTOS;

namespace Application.Contracts.IService;

public interface IAttachmentService
{
    Task<bool> ProcessMediaWebhookAsync(CloudinaryHookDto DTO);
}