using Application.Contracts.IService;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using Microsoft.AspNetCore.Http;

namespace Infra.Contracts_Imp;

  public class CloudinaryService: ICloudinaryService
  {
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(Cloudinary cloudinary)
    {
      _cloudinary = cloudinary;
    }
    public async Task<string> UploadPhotoAsync(IFormFile file)
    {
      if (file.Length == 0)
      {
        return null;
      }

      using var stream = file.OpenReadStream();

      var uploadParams = new ImageUploadParams()
      {
        File = new FileDescription(file.FileName, stream),
      };

      var uploadResult = await _cloudinary.UploadAsync(uploadParams);

      if (uploadResult.Error != null)
      {
        throw new Exception(uploadResult.Error.Message);
      }

      return uploadResult.SecureUrl.ToString();
    }
  }

