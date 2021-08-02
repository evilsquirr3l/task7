using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using xCloud.Task7.Helpers;
using xCloud.Task7.Interfaces;
using xCloud.Task7.Models;

namespace xCloud.Task7.Services
{
    public class BucketService : IBucketService
    {
        private readonly AppSettings _appSettings;

        public BucketService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        
        public async Task DeleteFileAsync(ImageMetadataModel image)
        {
            var credentials = new BasicAWSCredentials(_appSettings.AccessKey, _appSettings.SecretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.APSouth1
            };

            using var client = new AmazonS3Client(credentials, config);
            var fileTransferUtility = new TransferUtility(client);
            
            await fileTransferUtility.S3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _appSettings.BucketName,
                Key = image.Name
            });
        }
        
        public async Task<GetObjectResponse> DownloadFileAsync(ImageMetadataModel image)
        {
            var credentials = new BasicAWSCredentials(_appSettings.AccessKey, _appSettings.SecretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.APSouth1
            };
            
            using var client = new AmazonS3Client(credentials, config);
            var fileTransferUtility = new TransferUtility(client);

            var objectResponse = await fileTransferUtility.S3Client.GetObjectAsync(new GetObjectRequest()
            {
                BucketName = _appSettings.BucketName,
                Key = image.Name
            });

            return objectResponse;
        }
        
        public async Task<ImageMetadataModel> UploadFileToS3BucketAsync(IFormFile file)
        {
            var bucketName = !string.IsNullOrWhiteSpace(_appSettings.FolderName)
                ? _appSettings.BucketName + @"/" + _appSettings.FolderName
                : _appSettings.BucketName;

            var credentials = new BasicAWSCredentials(_appSettings.AccessKey, _appSettings.SecretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUCentral1
            };

            using var client = new AmazonS3Client(credentials, config);
            await using var newMemoryStream = new MemoryStream();
            await file.CopyToAsync(newMemoryStream);

            var fileExtension = Path.GetExtension(file.FileName);
            var imageName = $"{GenerateId()}{fileExtension}";

            // URL for Accessing Image
            var result = $"https://{bucketName}.s3.amazonaws.com/{imageName}";

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = imageName,
                BucketName = bucketName,
                CannedACL = S3CannedACL.PublicRead
            };

            var fileTransferUtility = new TransferUtility(client);
            await fileTransferUtility.UploadAsync(uploadRequest);

            return new ImageMetadataModel
            {
                UpdatedOn = DateTime.Now,
                Name = imageName,
                FileExtension = file.ContentType,
                SizeInBytes = file.Length
            };
        }
        
        private string GenerateId()
        {
            //{00000000-0000-0000-0000-000000000000}
            return Guid.NewGuid().ToString("N").ToUpper();
        }
    }
}