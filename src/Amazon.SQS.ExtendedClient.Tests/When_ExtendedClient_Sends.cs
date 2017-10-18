using System;

namespace Amazon.SQS.ExtendedClient.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Model;
    using Moq;
    using NUnit.Framework;
    using S3.Model;

    [TestFixture]
    public class When_ExtendedClient_Sends : ExtendedClientTestBase
    {
        [Test]
        public async Task Long_Message_Async_It_Is_Stored_In_S3()
        {
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await client.SendMessageAsync(messageRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            sqsMock.Verify(s => s.SendMessageAsync(It.Is<SendMessageRequest>(r => MessagePointerIsCorrect(r.MessageBody) && LargePayloadAttributeIsAdded(r.MessageAttributes)), default(CancellationToken)));
        }

        [Test]
        public async Task Long_Message_Async_It_Is_Not_Stored_In_S3_If_IsLargePayloadSupportEnabled_Is_False()
        {
            var extendedClient = new AmazonSQSExtendedClient(sqsMock.Object, new ExtendedClientConfiguration().WithLargePayloadSupportDisabled());
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await extendedClient.SendMessageAsync(messageRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Short_Message_Async_It_Is_Not_Stored_In_S3()
        {
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await client.SendMessageAsync(messageRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Short_Message_Async_It_Is_Stored_In_S3_If_AlwaysThroughS3_Configured()
        {
            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME).WithAlwaysThroughS3(true));
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await extendedClient.SendMessageAsync(messageRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            sqsMock.Verify(s => s.SendMessageAsync(It.Is<SendMessageRequest>(r => MessagePointerIsCorrect(r.MessageBody) && LargePayloadAttributeIsAdded(r.MessageAttributes)), default(CancellationToken)));
        }

        [Test]
        public async Task Short_Message_Async_It_Is_Stored_In_S3_If_Exceeds_Threshold()
        {
            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME).WithMessageSizeThreshold(100));
            var body = GenerateLongString(101);

            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await extendedClient.SendMessageAsync(messageRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            sqsMock.Verify(s => s.SendMessageAsync(It.Is<SendMessageRequest>(r => MessagePointerIsCorrect(r.MessageBody) && LargePayloadAttributeIsAdded(r.MessageAttributes)), default(CancellationToken)), Times.Once);
        }

        [Test]
        public async Task Long_Message_Async_S3KeyProvider_Is_Used_If_Configured()
        {
            var mockS3Provider = new Mock<IS3KeyPovider>();
            mockS3Provider.Setup(m => m.GenerateName()).Returns("CustomPrefix" + Guid.NewGuid().ToString("N"));

            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration()
                .WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME)
                .WithS3KeyProvider(mockS3Provider.Object));
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await extendedClient.SendMessageAsync(messageRequest);
            mockS3Provider.Verify(s => s.GenerateName(), Times.Once);
            s3Mock.Verify(s => s.PutObjectAsync(It.Is<PutObjectRequest>(r => r.Key.StartsWith("CustomPrefix")), It.IsAny<CancellationToken>()), Times.Once);
            sqsMock.Verify(s => s.SendMessageAsync(It.Is<SendMessageRequest>(r => MessagePointerIsCorrect(r.MessageBody, "CustomPrefix") && LargePayloadAttributeIsAdded(r.MessageAttributes)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}