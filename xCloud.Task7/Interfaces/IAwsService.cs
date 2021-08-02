using Amazon.S3;

namespace xCloud.Task7.Interfaces
{
    public interface IAwsService
    {
        AmazonS3Client GetBucketAccessClient();
    }
}