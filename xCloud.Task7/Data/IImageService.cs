using System.Collections.Generic;
using System.Threading.Tasks;
using xCloud.Task7.Models;

namespace xCloud.Task7.Data
{
    public interface IImageService
    {
        Task AddAsync(ImageMetadataModel image);

        Task<List<ImageMetadataModel>> GetImagesMetadata();
        
        Task DeleteAsync(ImageMetadataModel image);
        
        Task<List<ImageMetadataModel>> GetImageByImageName(string imageName);

        Task<ImageMetadataModel> GetImageByIdAsync(int id);
    }
}