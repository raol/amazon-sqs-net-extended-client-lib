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
            Is3KeyProvider = new GuidIs3KeyProvider();
            S3CannedACL = S3CannedACL.BucketOwnerFullControl;
        }

        public ExtendedClientConfiguration(ExtendedClientConfiguration other)
        {
            S3 = other.S3;
            S3BucketName = other.S3BucketName;
            IsLargePayloadSupportEnabled = other.IsLargePayloadSupportEnabled;
            AlwaysThroughS3 = other.AlwaysThroughS3;
            MessageSizeThreshold = other.MessageSizeThreshold;
            S3CannedACL = other.S3CannedACL;
        }

        public IAmazonS3 S3 { get; private set; }

        public string S3BucketName { get; private set; }

        public bool AlwaysThroughS3 { get; set; }

        public int MessageSizeThreshold { get; set; }

        public bool IsLargePayloadSupportEnabled { get; private set; }

        public IS3KeyProvider Is3KeyProvider { get; private set; }

        public S3CannedACL S3CannedACL { get; private set; }

        public bool RetainS3Messages { get; set; }

        public ExtendedClientConfiguration WithLargePayloadSupportEnabled(IAmazonS3 s3, string s3BucketName)
        {
            return WithLargePayloadSupportEnabled(s3, s3BucketName, S3CannedACL.BucketOwnerFullControl);
        }
        public ExtendedClientConfiguration WithLargePayloadSupportEnabled(IAmazonS3 s3, string s3BucketName, S3CannedACL s3CannedACL)
        {
            if (s3 == null)
            {
                throw new AmazonClientException("S3 client cannot be null");
            }

            if (string.IsNullOrWhiteSpace(s3BucketName))
            {
                throw new AmazonClientException("S3 bucket name cannot be null or empty");
            }

            if (s3CannedACL == null)
            {
                throw new AmazonClientException("S3 CannedACL cannot be null");
            }

            this.S3 = s3;
            this.S3BucketName = s3BucketName;
            this.IsLargePayloadSupportEnabled = true;
            this.S3CannedACL = s3CannedACL;

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
            this.AlwaysThroughS3 = alwaysThroughS3;
            return this;
        }

        public ExtendedClientConfiguration WithS3KeyProvider(IS3KeyProvider provider)
        {
            this.Is3KeyProvider = provider ?? throw new AmazonClientException("provider cannot be null");
            return this;
        }

        public ExtendedClientConfiguration WithRetainS3Messages(bool value)
        {
            this.RetainS3Messages = value;
            return this;
        }
    }
}