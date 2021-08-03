using System;
using xCloud.Task7.Models;

namespace xCloud.Task7.Services
{
    public class NotificationSubscriber : IObserver<ImageMetadataModel>
    {
        private IDisposable _unsubscriber;
        private IObservable<ImageMetadataModel> _provider;

        public NotificationSubscriber(IObservable<ImageMetadataModel> provider)
        {
            _provider = provider;
        }

        public virtual void Subscribe()
        {
            if (_provider is not null)
            {
                _unsubscriber = _provider.Subscribe(this);
            }
        }
        
        public virtual void OnCompleted()
        {
            Console.WriteLine("Done");
        }
        
        public virtual void OnError(Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        
        public virtual void OnNext(ImageMetadataModel image)
        {
            Console.WriteLine($"{image.Id}: {image.Name}");
        }
        
        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }
    }
}