using System.Threading.Tasks;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SQS;

namespace xCloud.Task7.Interfaces
{
    public interface IAwsService
    {
        AmazonS3Client GetBucketAccessClient();

        AmazonSimpleNotificationServiceClient GetSnsAccessClient();

        string GetSnsTopicArn();

        AmazonSQSClient GetSqsClient();

        Task<string> GetQueueUrl();
    }
}