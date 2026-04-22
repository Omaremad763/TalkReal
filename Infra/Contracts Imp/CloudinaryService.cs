using Application.Contracts.IService;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using Microsoft.AspNetCore.Http;

namespace Infra.Contracts_Imp;

public class CloudinaryService(Cloudinary cloudinary) : ICloudinaryService
{
    private readonly Cloudinary _cloudinary = cloudinary;

    public async Task<ImageUploadResult?> UploadFileAsync(IFormFile file)
    {
        if (file.Length == 0)
        {
            return null;
        }

        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "chat-attachments"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new InvalidOperationException($"Media Upload Failed: {uploadResult.Error.Message}");
        }

        return uploadResult;
    }
}

