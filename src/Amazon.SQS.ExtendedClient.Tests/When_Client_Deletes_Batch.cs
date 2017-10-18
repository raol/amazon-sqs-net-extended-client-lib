namespace Amazon.SQS.ExtendedClient.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class When_Client_Deletes_Batch : ExtendedClientTestBase
    {
        [Test]
        public async Task Long_Messages_Async_They_Are_Deleted_From_S3_And_SQS()
        {
            var s3Key = Guid.NewGuid().ToString("N");
            var longReceiptHandle = GenerateReceiptHandle(S3_BUCKET_NAME, s3Key, Constants.HandleTail);
            var entries = Enumerable.Repeat(0, 3).Select(_ => new DeleteMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), longReceiptHandle)).ToList();
            await client.DeleteMessageBatchAsync(new DeleteMessageBatchRequest(SQS_QUEUE_NAME, entries));
            s3Mock.Verify(m => m.DeleteObjectAsync(It.Is<string>(s => s.Equals(S3_BUCKET_NAME)), It.Is<string>(s => s.Equals(s3Key)), It.IsAny<CancellationToken>()), Times.Exactly(3));
            sqsMock.Verify(m => m.DeleteMessageBatchAsync(It.Is<DeleteMessageBatchRequest>(r => r.Entries.All(e => e.ReceiptHandle.Equals(Constants.HandleTail))), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Long_Messages_Async_They_Are_Deleted_From_SQS_Only_If_RetainS3Messages_Configured()
        {
            var extendedClient = new AmazonSQSExtendedClient(
                sqsMock.Object,
                new ExtendedClientConfiguration()
                    .WithLargePayloadSupportEnabled(s3Mock.Object, S3_BUCKET_NAME)
                    .WithRetainS3Messages(true));
            var s3Key = Guid.NewGuid().ToString("N");
            var longReceiptHandle = GenerateReceiptHandle(S3_BUCKET_NAME, s3Key, Constants.HandleTail);
            var entries = Enumerable.Repeat(0, 3).Select(_ => new DeleteMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), longReceiptHandle)).ToList();
            await extendedClient.DeleteMessageBatchAsync(new DeleteMessageBatchRequest(SQS_QUEUE_NAME, entries));
            s3Mock.Verify(m => m.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            sqsMock.Verify(m => m.DeleteMessageBatchAsync(It.Is<DeleteMessageBatchRequest>(r => r.Entries.All(e => e.ReceiptHandle.Equals(Constants.HandleTail))), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Short_Messages_Async_They_Are_Deleted_From_SQS_Only()
        {
            var entries = Enumerable.Repeat(0, 3).Select(_ => new DeleteMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), Constants.HandleTail)).ToList();
            await client.DeleteMessageBatchAsync(new DeleteMessageBatchRequest(SQS_QUEUE_NAME, entries));
            s3Mock.Verify(m => m.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            sqsMock.Verify(m => m.DeleteMessageBatchAsync(It.Is<DeleteMessageBatchRequest>(r => r.Entries.All(e => e.ReceiptHandle.Equals(Constants.HandleTail))), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}