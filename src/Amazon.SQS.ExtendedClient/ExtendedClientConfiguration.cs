namespace Amazon.SQS.ExtendedClient
{
    using Runtime;
    using S3;

    public class ExtendedClientConfiguration
    {
        public ExtendedClientConfiguration()
        {
            S3 = null;
            S3BucketName = null;
            AlwaysThroughS3 = false;
            MessageSizeThreshold = SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD;
            S3NamePovider = new GuidS3NameProvider();
        }

        public ExtendedClientConfiguration(ExtendedClientConfiguration other)
        {
            S3 = other.S3;
            S3BucketName = other.S3BucketName;
            IsLargePayloadSupportEnabled = other.IsLargePayloadSupportEnabled;
            AlwaysThroughS3 = other.AlwaysThroughS3;
            MessageSizeThreshold = other.MessageSizeThreshold;
        }

        public IAmazonS3 S3 { get; private set; }

        public string S3BucketName { get; private set; }

        public bool AlwaysThroughS3 { get; set; }

        public int MessageSizeThreshold { get; set; }

        public bool IsLargePayloadSupportEnabled { get; private set; }

        public IS3NamePovider S3NamePovider { get; private set; }

        public ExtendedClientConfiguration WithLargePayloadSupportEnabled(IAmazonS3 s3, string s3BucketName)
        {
            if (s3 == null)
            {
                throw new AmazonClientException("S3 client cannot be null");
            }

            if (string.IsNullOrWhiteSpace(s3BucketName))
            {
                throw new AmazonClientException("S3 bucket name cannot be null or empty");
            }

            this.S3 = s3;
            this.S3BucketName = s3BucketName;
            this.IsLargePayloadSupportEnabled = true;

            return this;
        }

        public void SetLargePayloadSupportDisabled()
        {
            S3 = null;
            S3BucketName = null;
            IsLargePayloadSupportEnabled = false;
        }

        public ExtendedClientConfiguration WithLargePayloadSupportDisabled()
        {
            this.SetLargePayloadSupportDisabled();
            return this;
        }

        public ExtendedClientConfiguration WithMessageSizeThreshold(int messageSizeThreshold)
        {
            this.MessageSizeThreshold = messageSizeThreshold;
            return this;
        }

        public ExtendedClientConfiguration WithAlwaysThroughS3(bool alwaysThroughS3)
        {
            this.AlwaysThroughS3 = true;
            return this;
        }

        public ExtendedClientConfiguration WithS3NameProvider(IS3NamePovider provider)
        {
            if (provider == null)
            {
                throw new AmazonClientException("provider cannot be null");
            }

            this.S3NamePovider = provider;
            return this;
        }
    }
}