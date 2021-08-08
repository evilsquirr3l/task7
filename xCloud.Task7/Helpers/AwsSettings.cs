namespace xCloud.Task7.Helpers
{
    public class AppSettings
    {
        public string BucketName { get; set; }
        
        public string AccessKey { get; set; }
        
        public string SecretKey { get; set; }
        
        public string FolderName { get; set; }

        public string Region { get; set; }
        
        public string SnsTopicArn { get; set; }

        public string QueueName { get; set; }
    }
}