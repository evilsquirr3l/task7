using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using xCloud.Task7.Data;
using xCloud.Task7.Interfaces;
using xCloud.Task7.Models;

namespace xCloud.Task7.Services
{
    public class ImageService : IImageService
    {
        private readonly BlobDbContext _blobDbContext;
        
        public ImageService(BlobDbContext blobDbContext)
        {
            _blobDbContext = blobDbContext;
        }

        public async Task AddMetadataToDatabaseAsync(ImageMetadataModel image)
        {
            await _blobDbContext.Images.AddAsync(image);
            await _blobDbContext.SaveChangesAsync();
        }

        public async Task<List<ImageMetadataModel>> GetImagesMetadataAsync()
        {
            return await _blobDbContext.Images.ToListAsync();
        }

        public async Task<List<ImageMetadataModel>> GetImagesMetadataByNameAsync(string imageName)
        {
            var imagesWithName = await _blobDbContext.Images
                .Where(x => x.Name.Equals(imageName))
                .ToListAsync();

            return imagesWithName;
        }

        public async Task<ImageMetadataModel> GetImageMetadataByIdAsync(int imageId)
        {
            return await _blobDbContext.Images.FindAsync(imageId);
        }

        public async Task<ImageMetadataModel> DeleteMetadataByIdAsync(int imageId)
        {
            var image = await _blobDbContext.Images.FindAsync(imageId);

            if (image is not null)
            {
                _blobDbContext.Remove(image);
            }
            
            await _blobDbContext.SaveChangesAsync();

            return image;
        }
    }
}