using Amazon.S3;
using Amazon.SimpleNotificationService;

namespace xCloud.Task7.Interfaces
{
    public interface IAwsService
    {
        AmazonS3Client GetBucketAccessClient();

        AmazonSimpleNotificationServiceClient GetSnsAccessClient();

        string GetSnsTopicArn();
    }
}