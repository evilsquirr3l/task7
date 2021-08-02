using System.Threading.Tasks;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using xCloud.Task7.Models;

namespace xCloud.Task7.Interfaces
{
    public interface IBucketService
    {
        Task DeleteFileAsync(ImageMetadataModel image);
        
        Task<GetObjectResponse> DownloadFileAsync(ImageMetadataModel image);
        
        Task<ImageMetadataModel> UploadFileToS3BucketAsync(IFormFile file);
    }
}