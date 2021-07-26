using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using xCloud.Task7.Data;
using xCloud.Task7.Helpers;
using xCloud.Task7.Models;

namespace xCloud.Task7.Controllers
{
    public class ImagesController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly IImageService _imageService;

        public ImagesController(IOptions<AppSettings> appSettings, IImageService imageService)
        {
            _imageService = imageService;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
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

                ImageMetadataModel documentStore = new ImageMetadataModel()
                {
                    UpdatedOn = DateTime.Now,
                    Name = imageName,
                    FileExtension = file.ContentType,
                    SizeInBytes = file.Length
                };

                await _imageService.AddAsync(documentStore);

            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null
                    && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }

            return RedirectToAction("AllFiles");
        }

        public async Task<IActionResult> AllFiles()
        {
            return View(await _imageService.GetImagesMetadata());
        }

        private string GenerateId()
        {
            //{00000000-0000-0000-0000-000000000000}
            return Guid.NewGuid().ToString("N").ToUpper();
        }

        public async Task<IActionResult> DownloadFileByName(int id)
        {
            try
            {
                var image = await _imageService.GetImageByIdAsync(id);
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

                if (objectResponse.ResponseStream == null)
                {
                    return NotFound();
                }
                
                return File(objectResponse.ResponseStream, objectResponse.Headers.ContentType, image.Name);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null
                    && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }

        }

        public async Task<IActionResult> DeleteFile(int id)
        {
            try
            {
                var image = await _imageService.GetImageByIdAsync(id);
                await _imageService.DeleteAsync(image);

                var credentials = new BasicAWSCredentials(_appSettings.AccessKey, _appSettings.SecretKey);
                var config = new AmazonS3Config
                {
                    RegionEndpoint = Amazon.RegionEndpoint.APSouth1
                };
                
                using var client = new AmazonS3Client(credentials, config);
                var fileTransferUtility = new TransferUtility(client);
                await fileTransferUtility.S3Client.DeleteObjectAsync(new DeleteObjectRequest()
                {
                    BucketName = _appSettings.BucketName,
                    Key = image.Name
                });

            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null
                    && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }
            return RedirectToAction("AllFiles");
        }
    }
}
