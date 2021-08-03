using System;
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
        private List<IObserver<ImageMetadataModel>> _observers;

        public ImageService(BlobDbContext blobDbContext)
        {
            _blobDbContext = blobDbContext;
            _observers = new List<IObserver<ImageMetadataModel>>();
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<ImageMetadataModel>> _observers;
            private IObserver<ImageMetadataModel> _observer;

            public Unsubscriber(List<IObserver<ImageMetadataModel>> observers,
                IObserver<ImageMetadataModel> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer is not null && _observers.Contains(_observer))
                {
                    _observers.Remove(_observer);
                }
            }
        }

        public IDisposable Subscribe(IObserver<ImageMetadataModel> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        public void Notify(ImageMetadataModel image)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(image);
            }
        }

        public async Task AddMetadataToDatabaseAsync(ImageMetadataModel image)
        {
            await _blobDbContext.Images.AddAsync(image);
            await _blobDbContext.SaveChangesAsync();
            Notify(image);
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