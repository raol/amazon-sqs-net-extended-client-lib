namespace Amazon.SQS.Net.ExtendedClient
{
    internal class MessageS3Pointer
    {
        public MessageS3Pointer()
        {
        }

        public MessageS3Pointer(string s3BucketName, string s3Key)
        {
            S3BucketName = s3BucketName;
            S3Key = s3Key;
        }

        public string S3Key { get; set; }

        public string S3BucketName { get; set; }
    }
}
