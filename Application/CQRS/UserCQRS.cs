using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.Contracts.IService;
using Application.DTOS;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace Application.CQRS;
public record AddUserPhotoCommand(Guid UserId, IFormFile File) : IRequest<bool>;
public record DeleteImageByIdCommand(Guid UserId) : IRequest<bool>;
public record GetUserImageQuery(Guid UserId) : IRequest<ImageDTO?>;
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

public class UserHandler(ITalkRealServices service,ICloudinaryService cloudinaryService) :
    IRequestHandler<AddUserPhotoCommand, bool>,
    IRequestHandler<GetUserImageQuery, ImageDTO?>,
    IRequestHandler<DeleteImageByIdCommand, bool>
{
    public async Task<bool> Handle(AddUserPhotoCommand request, CancellationToken cancellationToken)
    {
        var imageUrl = await cloudinaryService.UploadPhotoAsync(request.File);
        if (string.IsNullOrEmpty(imageUrl)) return false;
        return await service.UseService.UpdateProfileImageAsync(request.UserId, imageUrl);
    }
    public async Task<ImageDTO?> Handle(GetUserImageQuery request, CancellationToken cancellationToken)
    {
        var user = await service.UseService.GetUserByidAsync(request.UserId);
        if (user == null)  return null;
        return new ImageDTO{imageURL = user.ProfileImageUrl};
    }

    public async Task<bool> Handle(DeleteImageByIdCommand request, CancellationToken cancellationToken)
    {
       return await service.UseService.DeleteImagetById(request.UserId);
    }
}

