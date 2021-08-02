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

        public async Task AddAsync(ImageMetadataModel image)
        {
            await _blobDbContext.Images.AddAsync(image);
            await _blobDbContext.SaveChangesAsync();
        }

        public async Task<List<ImageMetadataModel>> GetImagesMetadata()
        {
            return await _blobDbContext.Images.ToListAsync();
        }

        public async Task<List<ImageMetadataModel>> GetImageByImageName(string imageName)
        {
            var imagesWithName = await _blobDbContext.Images
                .Where(x => x.Name.Equals(imageName))
                .ToListAsync();

            return imagesWithName;
        }

        public async Task<ImageMetadataModel> GetImageByIdAsync(int id)
        {
            return await _blobDbContext.Images.FindAsync(id);
        }

        public async Task<ImageMetadataModel> DeleteByIdAsync(int id)
        {
            var image = await _blobDbContext.Images.FindAsync(id);

            if (image is not null)
            {
                _blobDbContext.Remove(image);
            }
            
            await _blobDbContext.SaveChangesAsync();

            return image;
        }
    }
}