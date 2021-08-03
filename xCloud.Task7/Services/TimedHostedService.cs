using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using xCloud.Task7.Interfaces;

namespace xCloud.Task7.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ISqsService _sqsService;

        public TimedHostedService(ISqsService sqsService)
        {
            _sqsService = sqsService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //TODO: if this app scale horizontally, how many timers I'll get? :thinking:
            _timer = new Timer(ScheduledMethod, null, TimeSpan.Zero, 
                TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private void ScheduledMethod(object state)
        {
            _sqsService.SendMessageBatchRequest();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}