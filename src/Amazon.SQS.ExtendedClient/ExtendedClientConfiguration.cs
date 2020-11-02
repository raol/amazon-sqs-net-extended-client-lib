﻿namespace Amazon.SQS.ExtendedClient
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
        }

        public ExtendedClientConfiguration(ExtendedClientConfiguration other)
        {
            S3 = other.S3;
            S3BucketName = other.S3BucketName;
            IsLargePayloadSupportEnabled = other.IsLargePayloadSupportEnabled;
            AlwaysThroughS3 = other.AlwaysThroughS3;
            MessageSizeThreshold = other.MessageSizeThreshold;
            Is3KeyProvider = other.Is3KeyProvider;
        }

        public IAmazonS3 S3 { get; private set; }

        public string S3BucketName { get; private set; }

        public bool AlwaysThroughS3 { get; private set; }

        public long MessageSizeThreshold { get; private set; }

        public bool IsLargePayloadSupportEnabled { get; private set; }

        public IS3KeyProvider Is3KeyProvider { get; private set; }

        public bool RetainS3Messages { get; private set; }

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

        public ExtendedClientConfiguration WithMessageSizeThreshold(long messageSizeThreshold)
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