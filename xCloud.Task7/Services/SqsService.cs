using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
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

        public async Task SendSqsMessagesInBatchRequest()
        {
            using var sqsClient = _awsService.GetSqsClient();
            var queueUrl = await _awsService.GetQueueUrl();

            var listOfMessages = await GetMessages(sqsClient, queueUrl, 3, 5);

            var sendMessageBatchRequest = new SendMessageBatchRequest
            {
                Entries = listOfMessages.Select(message => new SendMessageBatchRequestEntry(message.MessageId, message.Body)).ToList(),
                QueueUrl = await _awsService.GetQueueUrl()
            };
            
            await sqsClient.SendMessageBatchAsync(sendMessageBatchRequest);
        }
        
        private static async Task<List<Message>> GetMessages(
            IAmazonSQS sqsClient, string qUrl, int waitTime = 3, int maxMessages = 10)
        {
            var messageResponse = await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest{
                QueueUrl = qUrl,
                MaxNumberOfMessages = maxMessages,
                WaitTimeSeconds = waitTime
            });
            
            return messageResponse.Messages;
        }
    }
}