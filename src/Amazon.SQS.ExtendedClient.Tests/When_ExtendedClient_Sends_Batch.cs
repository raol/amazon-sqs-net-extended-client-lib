namespace Amazon.SQS.ExtendedClient.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;
    using Moq;
    using NUnit.Framework;
    using S3.Model;

    [TestFixture]
    public class When_ExtendedClient_Sends_Batch : ExtendedClientTestBase
    {
#if NET45
        [Test]
        public void Long_Messages_All_Are_Stored_In_S3()
        {
            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            for (var i = 0; i < 3; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1)));
            }

            client.SendMessageBatch(batchRequest);

            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Exactly(3));
            sqsMock.Verify(
                s => s.SendMessageBatch(
                    It.Is<SendMessageBatchRequest>(r => r.Entries.All(e => MessagePointerIsCorrect(e.MessageBody) && LargePayloadAttributeIsAdded(e.MessageAttributes)))));
        }

        [Test]
        public void Long_Messages_S3KeyProvider_Is_Used_If_Configured()
        {
            var mockS3Provider = new Mock<IS3KeyProvider>();
            mockS3Provider.Setup(m => m.GenerateName()).Returns(Constants.CustomPrefix + Guid.NewGuid().ToString("N"));

            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration()
                .WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME)
                .WithS3KeyProvider(mockS3Provider.Object));

            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            for (var i = 0; i < 3; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1)));
            }

            extendedClient.SendMessageBatch(batchRequest);

            s3Mock.Verify(s => s.PutObject(It.Is<PutObjectRequest>(r => r.Key.StartsWith(Constants.CustomPrefix))), Times.Exactly(3));
            sqsMock.Verify(
                s => s.SendMessageBatch(
                    It.Is<SendMessageBatchRequest>(r => r.Entries.All(e => MessagePointerIsCorrect(e.MessageBody, Constants.CustomPrefix) && LargePayloadAttributeIsAdded(e.MessageAttributes)))));
        }
#endif

        [Test]
        public async Task Long_Message_Async_It_Is_Stored_In_S3()
        {
            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            for (var i = 0; i < 3; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1)));
            }

            await client.SendMessageBatchAsync(batchRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            sqsMock.Verify(s => s.SendMessageBatchAsync(It.Is<SendMessageBatchRequest>(r => r.Entries.All(e => MessagePointerIsCorrect(e.MessageBody) && LargePayloadAttributeIsAdded(e.MessageAttributes))), default(CancellationToken)));
        }

        [Test]
        public async Task Long_Message_Async_S3KeyProvider_Is_Used_If_Configured()
        {
            var mockS3Provider = new Mock<IS3KeyProvider>();
            mockS3Provider.Setup(m => m.GenerateName()).Returns(Constants.CustomPrefix + Guid.NewGuid().ToString("N"));

            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration()
                .WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME)
                .WithS3KeyProvider(mockS3Provider.Object));

            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            for (var i = 0; i < 3; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1)));
            }

            await extendedClient.SendMessageBatchAsync(batchRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.Is<PutObjectRequest>(r => r.Key.StartsWith(Constants.CustomPrefix)), It.IsAny<CancellationToken>()), Times.Exactly(3));
            sqsMock.Verify(s => s.SendMessageBatchAsync(It.Is<SendMessageBatchRequest>(r => r.Entries.All(e => MessagePointerIsCorrect(e.MessageBody, Constants.CustomPrefix) && LargePayloadAttributeIsAdded(e.MessageAttributes))), default(CancellationToken)));
        }

#if NET45
        [Test]
        public void Long_Message_It_Is_Not_Stored_In_S3_If_IsLargePayloadSupportEnabled_Is_False()
        {
            var extendedClient = new AmazonSQSExtendedClient(sqsMock.Object, new ExtendedClientConfiguration().WithLargePayloadSupportDisabled());
            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            for (var i = 0; i < 3; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), body));
            }

            extendedClient.SendMessageBatch(batchRequest);
            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Never);
            sqsMock.Verify(s => s.SendMessageBatch(It.Is<SendMessageBatchRequest>(r => r.Entries.All(e => e.MessageBody.Equals(body) && !e.MessageAttributes.ContainsKey(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME)))));
        }
#endif

        [Test]
        public async Task Long_Message_Async_It_Is_Not_Stored_In_S3_If_IsLargePayloadSupportEnabled_Is_False()
        {
            var extendedClient = new AmazonSQSExtendedClient(sqsMock.Object, new ExtendedClientConfiguration().WithLargePayloadSupportDisabled());
            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            for (var i = 0; i < 3; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), body));
            }

            await extendedClient.SendMessageBatchAsync(batchRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
            sqsMock.Verify(s => s.SendMessageBatchAsync(It.Is<SendMessageBatchRequest>(r => r.Entries.All(e => e.MessageBody.Equals(body) && !e.MessageAttributes.ContainsKey(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME))), It.IsAny<CancellationToken>()));
        }

#if NET45
        [Test]
        public void Short_Message_It_Is_Not_Stored_In_S3()
        {
            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD);
            for (var i = 0; i < 3; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), body));
            }

            client.SendMessageBatch(batchRequest);
            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Never);
            sqsMock.Verify(s => s.SendMessageBatch(It.Is<SendMessageBatchRequest>(r => r.Entries.All(e => e.MessageBody.Equals(body) && !e.MessageAttributes.ContainsKey(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME)))));
        }
#endif

        [Test]
        public async Task Short_Message_Async_It_Is_Not_Stored_In_S3()
        {
            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD);
            for (var i = 0; i < 3; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), body));
            }

            await client.SendMessageBatchAsync(batchRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
            sqsMock.Verify(s => s.SendMessageBatchAsync(It.Is<SendMessageBatchRequest>(r => r.Entries.All(e => e.MessageBody.Equals(body) && !e.MessageAttributes.ContainsKey(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME))), It.IsAny<CancellationToken>()));
        }

#if NET45
        [Test]
        public void Short_Message_It_Is_Stored_In_S3_If_AlwaysThroughS3_Configured()
        {
            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME).WithAlwaysThroughS3(true));
            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD);
            for (var i = 0; i < 3; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), body));
            }

            extendedClient.SendMessageBatch(batchRequest);
            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Exactly(3));
            sqsMock.Verify(
                s => s.SendMessageBatch(
                    It.Is<SendMessageBatchRequest>(r => r.Entries.All(e => MessagePointerIsCorrect(e.MessageBody) && LargePayloadAttributeIsAdded(e.MessageAttributes)))));
        }
#endif

        [Test]
        public async Task Short_Message_Async_It_Is_Stored_In_S3_If_AlwaysThroughS3_Configured()
        {
            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME).WithAlwaysThroughS3(true));
            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            var body = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD);
            for (var i = 0; i < 3; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), body));
            }

            await extendedClient.SendMessageBatchAsync(batchRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            sqsMock.Verify(
                s => s.SendMessageBatchAsync(
                    It.Is<SendMessageBatchRequest>(r => r.Entries.All(e => MessagePointerIsCorrect(e.MessageBody) && LargePayloadAttributeIsAdded(e.MessageAttributes))), It.IsAny<CancellationToken>()));
        }

#if NET45
        [Test]
        public void Short_Message_It_Is_Stored_In_S3_If_Exceeds_Threshold()
        {
            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME).WithMessageSizeThreshold(100));
            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            var body = GenerateLongString(101);
            for (var i = 0; i < 3; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), body));
            }

            extendedClient.SendMessageBatch(batchRequest);
            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Exactly(3));
            sqsMock.Verify(
                s => s.SendMessageBatch(
                    It.Is<SendMessageBatchRequest>(r => r.Entries.All(e => MessagePointerIsCorrect(e.MessageBody) && LargePayloadAttributeIsAdded(e.MessageAttributes)))));
        }
#endif

        [Test]
        public async Task Short_Message_Async_It_Is_Stored_In_S3_If_Exceeds_Threshold()
        {
            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME).WithMessageSizeThreshold(100));
            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            var body = GenerateLongString(101);
            for (var i = 0; i < 3; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), body));
            }

            await extendedClient.SendMessageBatchAsync(batchRequest);
            s3Mock.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            sqsMock.Verify(s => s.SendMessageBatchAsync(It.Is<SendMessageBatchRequest>(r => r.Entries.All(e => MessagePointerIsCorrect(e.MessageBody) && LargePayloadAttributeIsAdded(e.MessageAttributes))), default(CancellationToken)));
        }

#if NET45
        [Test]
        public void Short_And_Long_Messages_Only_Long_Stored_In_S3()
        {
            var batchRequest = new SendMessageBatchRequest(SQS_QUEUE_NAME, new List<SendMessageBatchRequestEntry>());
            var shortBody = "body";
            var longBody = GenerateLongString(SQSExtendedClientConstants.DEFAULT_MESSAGE_SIZE_THRESHOLD + 1);
            for (var i = 0; i < 4; i++)
            {
                batchRequest.Entries.Add(new SendMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), i % 2 == 0 ? shortBody : longBody));
            }

            client.SendMessageBatch(batchRequest);

            s3Mock.Verify(s => s.PutObject(It.IsAny<PutObjectRequest>()), Times.Exactly(2));
            sqsMock.Verify(
                s => s.SendMessageBatch(
                    It.Is<SendMessageBatchRequest>(r =>
                        r.Entries.Zip(Enumerable.Range(0, 4), (e, i) => new { Id = i, Entry = e })
                        .Where(t => t.Id % 2 == 0)
                        .Select(t => t.Entry)
                        .All(e => e.MessageBody.Equals(shortBody) && !e.MessageAttributes.ContainsKey(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME)) &&
                        r.Entries.Zip(Enumerable.Range(0, 4), (e, i) => new { Id = i, Entry = e })
                        .Where(t => t.Id % 2 != 0)
                        .Select(t => t.Entry)
                        .All(e => MessagePointerIsCorrect(e.MessageBody) && LargePayloadAttributeIsAdded(e.MessageAttributes)))));
        }
#endif
    }
}