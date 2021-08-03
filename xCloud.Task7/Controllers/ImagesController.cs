using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using xCloud.Task7.Interfaces;

namespace xCloud.Task7.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IBucketService _bucketService;
        private readonly ISqsService _sqsService;

        public ImagesController(IImageService imageService, IBucketService bucketService, ISqsService sqsService)
        {
            _imageService = imageService;
            _bucketService = bucketService;
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
            var image = await _bucketService.UploadFileToS3BucketAsync(file);

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
            var bucketObjectResponse = await _bucketService.DownloadFileAsync(image);

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

            await _bucketService.DeleteFileAsync(imageName);
            
            return RedirectToAction("AllFiles");
        }
    }
}