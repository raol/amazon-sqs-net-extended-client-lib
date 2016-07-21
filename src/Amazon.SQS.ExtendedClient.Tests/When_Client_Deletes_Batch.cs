namespace Amazon.SQS.ExtendedClient.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using Model;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class When_Client_Deletes_Batch : ExtendedClientTestBase
    {
        [Test]
        public void Long_Messages_They_Are_Deleted_From_S3_And_SQS()
        {
            var handleTail = "_handle_";
            var s3Key = Guid.NewGuid().ToString("N");
            var longReceiptHandle = GenerateReceiptHandle(S3_BUCKET_NAME, s3Key, handleTail);
            var entries = Enumerable.Repeat(0, 3).Select(_ => new DeleteMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), longReceiptHandle)).ToList();
            client.DeleteMessageBatch(new DeleteMessageBatchRequest(SQS_QUEUE_NAME, entries));
            s3Mock.Verify(m => m.DeleteObject(It.Is<string>(s => s.Equals(S3_BUCKET_NAME)), It.Is<string>(s => s.Equals(s3Key))), Times.Exactly(3));
            sqsMock.Verify(m => m.DeleteMessageBatch(It.Is<DeleteMessageBatchRequest>(r => r.Entries.All(e => e.ReceiptHandle.Equals(handleTail)))), Times.Once);
        }

        [Test]
        public async void Long_Messages_Async_They_Are_Deleted_From_S3_And_SQS()
        {
            var handleTail = "_handle_";
            var s3Key = Guid.NewGuid().ToString("N");
            var longReceiptHandle = GenerateReceiptHandle(S3_BUCKET_NAME, s3Key, handleTail);
            var entries = Enumerable.Repeat(0, 3).Select(_ => new DeleteMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), longReceiptHandle)).ToList();
            await client.DeleteMessageBatchAsync(new DeleteMessageBatchRequest(SQS_QUEUE_NAME, entries));
            s3Mock.Verify(m => m.DeleteObjectAsync(It.Is<string>(s => s.Equals(S3_BUCKET_NAME)), It.Is<string>(s => s.Equals(s3Key)), It.IsAny<CancellationToken>()), Times.Exactly(3));
            sqsMock.Verify(m => m.DeleteMessageBatchAsync(It.Is<DeleteMessageBatchRequest>(r => r.Entries.All(e => e.ReceiptHandle.Equals(handleTail))), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Short_Messages_They_Are_Deleted_From_SQS_Only()
        {
            var handleTail = "_handle_";
            var entries = Enumerable.Repeat(0, 3).Select(_ => new DeleteMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), handleTail)).ToList();
            client.DeleteMessageBatch(new DeleteMessageBatchRequest(SQS_QUEUE_NAME, entries));
            s3Mock.Verify(m => m.DeleteObject(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            sqsMock.Verify(m => m.DeleteMessageBatch(It.Is<DeleteMessageBatchRequest>(r => r.Entries.All(e => e.ReceiptHandle.Equals(handleTail)))), Times.Once);
        }

        [Test]
        public async void Short_Messages_Async_They_Are_Deleted_From_SQS_Only()
        {
            var handleTail = "_handle_";
            var entries = Enumerable.Repeat(0, 3).Select(_ => new DeleteMessageBatchRequestEntry(Guid.NewGuid().ToString("N"), handleTail)).ToList();
            await client.DeleteMessageBatchAsync(new DeleteMessageBatchRequest(SQS_QUEUE_NAME, entries));
            s3Mock.Verify(m => m.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            sqsMock.Verify(m => m.DeleteMessageBatchAsync(It.Is<DeleteMessageBatchRequest>(r => r.Entries.All(e => e.ReceiptHandle.Equals(handleTail))), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}