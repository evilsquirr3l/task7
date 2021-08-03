using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Options;
using xCloud.Task7.Helpers;
using xCloud.Task7.Interfaces;

namespace xCloud.Task7.Services
{
    public class AwsService : IAwsService
    {
        private readonly AppSettings _appSettings;

        public AwsService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        
        public AmazonS3Client GetBucketAccessClient()
        {
            var credentials = GetAwsCredentials();
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_appSettings.Region)
            };

            return new AmazonS3Client(credentials, config);
        }

        public AmazonSimpleNotificationServiceClient GetSnsAccessClient()
        {
            var credentials = GetAwsCredentials();
            var config = new AmazonSimpleNotificationServiceConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_appSettings.Region)
            };

            return new AmazonSimpleNotificationServiceClient(credentials, config);
        }

        private BasicAWSCredentials GetAwsCredentials()
        {
            return new BasicAWSCredentials(_appSettings.AccessKey, _appSettings.SecretKey);
        }

        public string GetSnsTopicArn()
        {
            return _appSettings.TopicArn;
        }
    }
}