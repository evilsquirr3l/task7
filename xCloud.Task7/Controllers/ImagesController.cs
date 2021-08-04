using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using xCloud.Task7.Interfaces;

namespace xCloud.Task7.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IS3Service _s3Service;
        private readonly ISqsService _sqsService;

        public ImagesController(IImageService imageService, IS3Service S3Service, ISqsService sqsService)
        {
            _imageService = imageService;
            _s3Service = S3Service;
            _sqsService = sqsService;
        }

        [HttpGet]
        public IActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var image = await _s3Service.UploadFileToS3BucketAsync(file);

            await _imageService.AddMetadataToDatabaseAsync(image);
            
            var message = @$"Image with name {image.Name}, extension {image.FileExtension} 
                            and size {image.SizeInBytes} was uploaded! 
                            You can download it here: {Request.Path.Value}";
            
            await _sqsService.PublishEventToSqsQueue(message);

            return RedirectToAction("AllFiles");
        }

        public async Task<IActionResult> AllFiles()
        {
            return View(await _imageService.GetImagesMetadataAsync());
        }

        public async Task<IActionResult> DownloadFileAsync(int id)
        {
            var image = await _imageService.GetImageMetadataByIdAsync(id);
            var bucketObjectResponse = await _s3Service.DownloadFileAsync(image);

            if (image is null)
            {
                return NotFound();
            }

            return File(bucketObjectResponse.ResponseStream, bucketObjectResponse.Headers.ContentType,
                bucketObjectResponse.Key);
        }

        public async Task<IActionResult> DeleteFile(int id)
        {
            var imageName = await _imageService.DeleteMetadataByIdAsync(id);

            await _s3Service.DeleteFileAsync(imageName);
            
            return RedirectToAction("AllFiles");
        }
    }
}