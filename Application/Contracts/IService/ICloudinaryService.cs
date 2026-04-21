using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudinaryDotNet.Actions;

using Microsoft.AspNetCore.Http;

namespace Application.Contracts.IService;
public interface ICloudinaryService
{
    Task<ImageUploadResult?> UploadFileAsync(IFormFile file);

}
