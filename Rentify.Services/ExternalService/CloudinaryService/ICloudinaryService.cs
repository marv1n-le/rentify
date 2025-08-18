using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Rentify.Repositories.Helper;

namespace Rentify.Services.ExternalService.CloudinaryService;

public interface ICloudinaryService
{
    Task<DeletionResult> DeletePhotoAsync(string publicId);
    Task<PhotoUploadResult> AddPhotoAsync(IFormFile file);
    string GetCloudinaryPublicIdFromUrl(string imageUrl);
}