using CloudinaryDotNet.Actions;

using Microsoft.AspNetCore.Http;

namespace Application.Contracts.IService;
public interface ICloudinaryService
{
    Task<ImageUploadResult?> UploadFileAsync(IFormFile file);

}
