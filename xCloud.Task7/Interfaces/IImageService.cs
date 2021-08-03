using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using xCloud.Task7.Models;

namespace xCloud.Task7.Interfaces
{
    public interface IImageService : IObservable<ImageMetadataModel>
    {
        Task AddMetadataToDatabaseAsync(ImageMetadataModel image);

        Task<List<ImageMetadataModel>> GetImagesMetadataAsync();
        
        Task<ImageMetadataModel> DeleteMetadataByIdAsync(int imageId);
        
        Task<List<ImageMetadataModel>> GetImagesMetadataByNameAsync(string imageName);

        Task<ImageMetadataModel> GetImageMetadataByIdAsync(int imageId);
    }
}