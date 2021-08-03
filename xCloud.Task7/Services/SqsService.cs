using System.Threading.Tasks;
using Amazon.SQS.Model;
using xCloud.Task7.Interfaces;

namespace xCloud.Task7.Services
{
    public class SqsService : ISqsService
    {
        private readonly IAwsService _awsService;

        public SqsService(IAwsService awsService)
        {
            _awsService = awsService;
        }

        public async Task<SendMessageResponse> PublishEventToSqsQueue(string message)
        {
            using var sqsClient = _awsService.GetSqsClient();

            var queueUrl = await _awsService.GetQueueUrl();
            var request = new SendMessageRequest(queueUrl, message);

            return await sqsClient.SendMessageAsync(request);
        }
    }
}