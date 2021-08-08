using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;
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

        public async Task SendSqsMessagesToSnsTopic()
        {
            using var sqsClient = _awsService.GetSqsClient();
            using var snsClient = _awsService.GetSnsAccessClient();
            var queueUrl = await _awsService.GetQueueUrl();

            var listOfMessages = await GetMessages(sqsClient, queueUrl, 3, 5);

            var numberOfMessages = listOfMessages.Count;
            if (numberOfMessages == 0)
            {
                return;
            }
            
            var emailBody = "";
            for (int i = 0; i < numberOfMessages; i++)
            {
                var messageNumber = i + 1;
                emailBody += @$"You have unread messages: {messageNumber}. {listOfMessages[i].Body}";
            }

            var publishRequest = new PublishRequest(_awsService.GetSnsTopicArn(), emailBody);
            
            await snsClient.PublishAsync(publishRequest);
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