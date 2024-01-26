using System;

namespace Amazon.SQS.ExtendedClient.Tests
{
    using Model;
    using Moq;
    using NUnit.Framework;
    using S3.Model;
    using System.Threading;
    using System.Threading.Tasks;

    [TestFixture]
    public class When_ExtendedClient_Sends : ExtendedClientTestBase
    {
#if NET45
        [Test]
        public void Long_Message_It_Is_Stored_In_S3()
        {
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            client.SendMessage(messageRequest);
            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Once);
            sqsMock.Verify(s => s.SendMessage(It.Is<SendMessageRequest>(r => MessagePointerIsCorrect(r.MessageBody) && LargePayloadAttributeIsAdded(r.MessageAttributes))));
        }
#endif

        [Test]
        public async Task Long_Message_Async_It_Is_Stored_In_S3()
        {
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await client.SendMessageAsync(messageRequest);
            s3Mock.Verify(s =>
                s.PutObjectAsync(It.Is<PutObjectRequest>(pm
                        => pm.ContentBody == body
                        && pm.BucketName == S3_BUCKET_NAME
                        && pm.CannedACL == S3.S3CannedACL.BucketOwnerFullControl),
                    It.IsAny<CancellationToken>()), Times.Once);
            sqsMock.Verify(s => s.SendMessageAsync(It.Is<SendMessageRequest>(r => MessagePointerIsCorrect(r.MessageBody) && LargePayloadAttributeIsAdded(r.MessageAttributes)), default(CancellationToken)));
        }
#if NET45
        [Test]
        public void Long_Message_It_Is_Not_Stored_In_S3_If_IsLargePayloadSupportEnabled_Is_False()
        {
            var extendedClient = new AmazonSQSExtendedClient(sqsMock.Object, new ExtendedClientConfiguration().WithLargePayloadSupportDisabled());
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            extendedClient.SendMessage(messageRequest);
            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Never);
            sqsMock.Verify(s => s.SendMessage(It.Is<SendMessageRequest>(r => r.MessageBody.Equals(body))));
        }
#endif

        [Test]
        public async Task Long_Message_Async_It_Is_Not_Stored_In_S3_If_IsLargePayloadSupportEnabled_Is_False()
        {
            var extendedClient = new AmazonSQSExtendedClient(sqsMock.Object, new ExtendedClientConfiguration().WithLargePayloadSupportDisabled());
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await extendedClient.SendMessageAsync(messageRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
#if NET45
        [Test]
        public void Short_Message_It_Is_Not_Stored_In_S3()
        {
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            client.SendMessage(messageRequest);
            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Never);
        }
#endif

        [Test]
        public async Task Short_Message_Async_It_Is_Not_Stored_In_S3()
        {
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await client.SendMessageAsync(messageRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
#if NET45
        [Test]
        public void Short_Message_It_Is_Stored_In_S3_If_AlwaysThroughS3_Configured()
        {
            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME).WithAlwaysThroughS3(true));
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            extendedClient.SendMessage(messageRequest);
            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Once);
            sqsMock.Verify(s => s.SendMessage(It.Is<SendMessageRequest>(r => MessagePointerIsCorrect(r.MessageBody) && LargePayloadAttributeIsAdded(r.MessageAttributes))));
        }
#endif
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
#if NET45
        [Test]
        public void Short_Message_It_Is_Stored_In_S3_If_Exceeds_Threshold()
        {
            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME).WithMessageSizeThreshold(100));
            var body = GenerateLongString(101);

            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            extendedClient.SendMessage(messageRequest);
            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Once);
            sqsMock.Verify(s => s.SendMessage(It.Is<SendMessageRequest>(r => MessagePointerIsCorrect(r.MessageBody) && LargePayloadAttributeIsAdded(r.MessageAttributes))));
        }
#endif

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
#if NET45
        [Test]
        public void Long_Message_S3KeyProvider_Is_Used_If_Configured()
        {
            var mockS3Provider = new Mock<IS3KeyProvider>();
            mockS3Provider.Setup(m => m.GenerateName()).Returns(Constants.CustomPrefix + Guid.NewGuid().ToString("N"));

            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration()
                .WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME)
                .WithS3KeyProvider(mockS3Provider.Object));
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            extendedClient.SendMessage(messageRequest);
            mockS3Provider.Verify(s => s.GenerateName(), Times.Once);
            s3Mock.Verify(s => s.PutObject(It.Is<PutObjectRequest>(r => r.Key.StartsWith(Constants.CustomPrefix))), Times.Once);
            sqsMock.Verify(s => s.SendMessage(It.Is<SendMessageRequest>(r => MessagePointerIsCorrect(r.MessageBody, Constants.CustomPrefix) && LargePayloadAttributeIsAdded(r.MessageAttributes))), Times.Once);
        }
#endif

        [Test]
        public async Task Long_Message_Async_S3KeyProvider_Is_Used_If_Configured()
        {
            var mockS3Provider = new Mock<IS3KeyProvider>();
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