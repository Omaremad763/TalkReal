using Application.Contracts;
using Application.Contracts.IService;
using Application.DTOS;

using Domain.Value_Object;

using FluentValidation;

using MediatR;

namespace Application.CQRS;
public record ProcessWebhookCommand(CloudinaryHookDTO DTO) : IRequest<bool>;
public class ProcessWebhookCommandValidator : AbstractValidator<ProcessWebhookCommand>
{
    public ProcessWebhookCommandValidator()
    {
        RuleFor(x => x.DTO.PublicId).NotEmpty().WithMessage("PublicId is required to identify the attachment.");
        RuleFor(x => x.DTO.SecureUrl).NotEmpty().Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("A valid secure URL is required.");
    }
}
public class WebhookHandler(ITalkRealServices service, ICloudinaryService cloudinaryService) :
    IRequestHandler<ProcessWebhookCommand, bool>
{

    public async Task<bool> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
    {
        return await service.AttachmentService.ProcessMediaWebhookAsync(request.DTO);
    }
}
