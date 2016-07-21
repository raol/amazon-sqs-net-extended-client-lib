namespace Amazon.SQS.Net.ExtendedClient.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Model;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using S3;
    using S3.Model;

    [TestFixture]
    public class When_ExtendedClient_Sends : ExtendedClientTestBase
    {
        [Test]
        public void Long_Message_It_Is_Stored_In_S3()
        {
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            client.SendMessage(messageRequest);
            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Once);
            sqsMock.Verify(s => s.SendMessage(It.Is<SendMessageRequest>(r => MessagePointerIsCorrect(r.MessageBody) && LargePayloadAttributeIsAdded(r.MessageAttributes))));
        }

        [Test]
        public async void Long_Message_Async_It_Is_Stored_In_S3()
        {
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await client.SendMessageAsync(messageRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            sqsMock.Verify(s => s.SendMessageAsync(It.Is<SendMessageRequest>(r => MessagePointerIsCorrect(r.MessageBody) && LargePayloadAttributeIsAdded(r.MessageAttributes)), default(CancellationToken)));
        }

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

        [Test]
        public async void Long_Message_Async_It_Is_Not_Stored_In_S3_If_IsLargePayloadSupportEnabled_Is_False()
        {
            var extendedClient = new AmazonSQSExtendedClient(sqsMock.Object, new ExtendedClientConfiguration().WithLargePayloadSupportDisabled());
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await extendedClient.SendMessageAsync(messageRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void Short_Message_It_Is_Not_Stored_In_S3()
        {
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            client.SendMessage(messageRequest);
            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Never);
        }

        [Test]
        public async void Short_Message_Async_It_Is_Not_Stored_In_S3()
        {
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD);
            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await client.SendMessageAsync(messageRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

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

        [Test]
        public async void Short_Message_Async_It_Is_Stored_In_S3_If_AlwaysThroughS3_Configured()
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

        [Test]
        public async void Short_Message_Async_It_Is_Stored_In_S3_If_Exceeds_Threshold()
        {
            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME).WithMessageSizeThreshold(100));
            var body = GenerateLongString(101);

            var messageRequest = new SendMessageRequest(SQS_QUEUE_NAME, body);
            await extendedClient.SendMessageAsync(messageRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            sqsMock.Verify(s => s.SendMessageAsync(It.Is<SendMessageRequest>(r => MessagePointerIsCorrect(r.MessageBody) && LargePayloadAttributeIsAdded(r.MessageAttributes)), default(CancellationToken)));
        }
    }
}