using System;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using xCloud.Task7.Interfaces;

namespace xCloud.Task7.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IBucketService _bucketService;

        public ImagesController(IImageService imageService, IBucketService bucketService)
        {
            _imageService = imageService;
            _bucketService = bucketService;
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
                var image = await _bucketService.UploadFileToS3BucketAsync(file);
                
                await _imageService.AddMetadataToDatabaseAsync(image);
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
            return View(await _imageService.GetImagesMetadataAsync());
        }

        public async Task<IActionResult> DownloadFileAsync(int id)
        {
            try
            {
                var image = await _imageService.GetImageMetadataByIdAsync(id);
                var bucketObjectResponse = await _bucketService.DownloadFileAsync(image);
                
                if (bucketObjectResponse == null)
                {
                    return NotFound();
                }
                
                return File(bucketObjectResponse.ResponseStream, bucketObjectResponse.Headers.ContentType, bucketObjectResponse.Key);
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
                var imageName = await _imageService.DeleteMetadataByIdAsync(id);
                
                await _bucketService.DeleteFileAsync(imageName);
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
