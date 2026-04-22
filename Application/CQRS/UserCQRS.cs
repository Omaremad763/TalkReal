using Application.Contracts;
using Application.Contracts.IService;
using Application.DTOS;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace Application.CQRS;
public record AddUserPhotoCommand(Guid UserId, IFormFile File) : IRequest<bool>;
public record DeleteImageByIdCommand(Guid UserId) : IRequest<bool>;
public record GetUserImageQuery(Guid UserId) : IRequest<ImageDto?>;
public class AddUserPhotoValidator : AbstractValidator<AddUserPhotoCommand>
{
    public AddUserPhotoValidator()
    {
        RuleFor(x => x.UserId).NotNull();

        RuleFor(x => x.File)
            .NotNull().WithMessage("photo reqired.")
            .Must(file => file.Length > 0).WithMessage("empty fiel.")
            .Must(file => file.Length <= 5 * 1024 * 1024).WithMessage("max size is 5MB.")
            .Must(file => IsSupportedImage(file.ContentType)).WithMessage("jpg and png are only supported");
    }

    private static bool IsSupportedImage(string contentType)
    {
        var supportedTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
        return supportedTypes.Contains(contentType.ToLower());
    }
}
public class DeleteImagetByIdValidator : AbstractValidator<AddUserPhotoCommand>
{
    public DeleteImagetByIdValidator()
    {
        RuleFor(x => x.UserId).NotNull();
    }

}

public class UserHandler(ITalkRealServices service, ICloudinaryService cloudinaryService) :
    IRequestHandler<AddUserPhotoCommand, bool>,
    IRequestHandler<GetUserImageQuery, ImageDto?>,
    IRequestHandler<DeleteImageByIdCommand, bool>
{
    public async Task<bool> Handle(AddUserPhotoCommand request, CancellationToken cancellationToken)
    {
        var imageUrl = await cloudinaryService.UploadFileAsync(request.File);
        if (imageUrl is null) return false;
        return await service.UseService.UpdateProfileImageAsync(request.UserId, imageUrl.SecureUrl.ToString());
    }
    public async Task<ImageDto?> Handle(GetUserImageQuery request, CancellationToken cancellationToken)
    {
        var user = await service.UseService.GetUserByidAsync(request.UserId);
        if (user is null) return null;
        return new ImageDto { ImageURL = user.ProfileImageUrl };
    }

    public async Task<bool> Handle(DeleteImageByIdCommand request, CancellationToken cancellationToken)
    {
        return await service.UseService.DeleteImagetById(request.UserId);
    }
}

