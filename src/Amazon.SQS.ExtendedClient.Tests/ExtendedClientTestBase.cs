namespace Amazon.SQS.ExtendedClient.Tests
{
    using Model;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using S3;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ExtendedClientTestBase
    {
        protected Mock<IAmazonS3> s3Mock;

        protected Mock<IAmazonSQS> sqsMock;

        protected AmazonSQSExtendedClient client;

        protected const string S3_BUCKET_NAME = "test-bucket-name";

        protected const string SQS_QUEUE_NAME = "test-queue-url";

        [SetUp]
        public void SetUp()
        {
            s3Mock = new Mock<IAmazonS3>();
            sqsMock = new Mock<IAmazonSQS>();
            client = new AmazonSQSExtendedClient(sqsMock.Object, new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME));
        }

        protected string GenerateLongString(int threshold)
        {
            return new string(Enumerable.Repeat('x', threshold).ToArray());
        }

        protected bool MessagePointerIsCorrect(string messagePointerBody)
        {
            var pointer = JsonConvert.DeserializeObject<MessageS3Pointer>(messagePointerBody);

            return pointer.S3BucketName == S3_BUCKET_NAME && Guid.Parse(pointer.S3Key) != Guid.Empty;
        }

        protected bool MessagePointerIsCorrect(string messagePointerBody, string customPrefix)
        {
            if (string.IsNullOrEmpty(customPrefix))
            {
                Assert.Fail("customPrefix is not specified.");
            }

            var pointer = JsonConvert.DeserializeObject<MessageS3Pointer>(messagePointerBody);

            return pointer.S3BucketName == S3_BUCKET_NAME &&
                   pointer.S3Key.StartsWith(customPrefix) &&
                   Guid.Parse(pointer.S3Key.Replace(customPrefix, string.Empty)) != Guid.Empty;

        }

        protected bool LargePayloadAttributeIsAdded(Dictionary<string, MessageAttributeValue> attributes)
        {
            MessageAttributeValue value;
            if (!attributes.TryGetValue(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, out value))
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(value.StringValue);
        }

        protected string GenerateReceiptHandle(string s3BucketName, string s3Key, string receiptHandle)
        {
            return string.Concat(
               SQSExtendedClientConstants.S3_BUCKET_NAME_MARKER,
               s3BucketName,
               SQSExtendedClientConstants.S3_BUCKET_NAME_MARKER,
               SQSExtendedClientConstants.S3_KEY_MARKER,
               s3Key,
               SQSExtendedClientConstants.S3_KEY_MARKER,
               receiptHandle);
        }
    }
}