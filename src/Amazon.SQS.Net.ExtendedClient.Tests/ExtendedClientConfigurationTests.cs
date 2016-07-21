namespace Amazon.SQS.Net.ExtendedClient.Tests
{
    using Moq;
    using NUnit.Framework;
    using S3;

    [TestFixture]
    public class ExtendedClientConfigurationTests
    {
        private readonly string s3BucketName = "test-s3-bucket";
        
        [Test]
        public void TestLargePayloadSupportEnabled()
        {
            var s3Mock = new Mock<IAmazonS3>();
            var clientConfig = new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Mock.Object, s3BucketName);
            Assert.IsTrue(clientConfig.IsLargePayloadSupportEnabled);
            Assert.NotNull(clientConfig.S3);
            Assert.IsNotEmpty(clientConfig.S3BucketName);
        }

        [Test]
        public void TestLargePayloadSupportDisabled()
        {
            var s3Mock = new Mock<IAmazonS3>();
            var clientConfig = new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Mock.Object, s3BucketName);
            Assert.IsTrue(clientConfig.IsLargePayloadSupportEnabled);
            Assert.NotNull(clientConfig.S3);
            Assert.IsNotEmpty(clientConfig.S3BucketName);
            clientConfig = clientConfig.WithLargePayloadSupportDisabled();
            Assert.IsFalse(clientConfig.IsLargePayloadSupportEnabled);
            Assert.Null(clientConfig.S3);
            Assert.IsNull(clientConfig.S3BucketName);
        }

        [Test]
        public void TestAlwaysThroughS3()
        {
            var clientConfig = new ExtendedClientConfiguration().WithAlwaysThroughS3(true);
            Assert.IsTrue(clientConfig.AlwaysThroughS3);
        }

        [Test]
        public void TestMessageSizeThreshold()
        {
            var clientConfig = new ExtendedClientConfiguration().WithMessageSizeThreshold(100);
            Assert.AreEqual(100, clientConfig.MessageSizeThreshold);
        }
    }
}