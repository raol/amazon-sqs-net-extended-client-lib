using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using Moq;
using NUnit.Framework;

namespace Amazon.SQS.ExtendedClient.Tests
{
    public class When_Changing_Message_Visibility : ExtendedClientTestBase
    {
        private const int VisibilityTimeout = 100;

        [Test]
        public async Task Short_messages_Async()
        {
            await client.ChangeMessageVisibilityAsync(new ChangeMessageVisibilityRequest(SQS_QUEUE_NAME, Constants.HandleTail, VisibilityTimeout));
            sqsMock.Verify(x => x.ChangeMessageVisibilityAsync(It.Is<ChangeMessageVisibilityRequest>(r =>
                    r.VisibilityTimeout == VisibilityTimeout &&
                    r.QueueUrl == SQS_QUEUE_NAME &&
                    r.ReceiptHandle == Constants.HandleTail),
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Test]
        public async Task Long_messages_Async()
        {
            var s3Key = Guid.NewGuid().ToString("N");
            var longReceiptHandle = GenerateReceiptHandle(S3_BUCKET_NAME, s3Key, Constants.HandleTail);
            await client.ChangeMessageVisibilityAsync(new ChangeMessageVisibilityRequest(SQS_QUEUE_NAME, longReceiptHandle, VisibilityTimeout));
            sqsMock.Verify(x => x.ChangeMessageVisibilityAsync(It.Is<ChangeMessageVisibilityRequest>(r =>
                    r.VisibilityTimeout == VisibilityTimeout &&
                    r.QueueUrl == SQS_QUEUE_NAME &&
                    r.ReceiptHandle == Constants.HandleTail),
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }
#if NET45

        [Test]
        public void Short_messages()
        {
            client.ChangeMessageVisibility(new ChangeMessageVisibilityRequest(SQS_QUEUE_NAME, Constants.HandleTail, VisibilityTimeout));
            sqsMock.Verify(x => x.ChangeMessageVisibility(It.Is<ChangeMessageVisibilityRequest>(r =>
                r.VisibilityTimeout == VisibilityTimeout &&
                r.QueueUrl == SQS_QUEUE_NAME &&
                r.ReceiptHandle == Constants.HandleTail)), 
                Times.Once);
        }

        [Test]
        public void Long_messages()
        {
            var s3Key = Guid.NewGuid().ToString("N");
            var longReceiptHandle = GenerateReceiptHandle(S3_BUCKET_NAME, s3Key, Constants.HandleTail);
            client.ChangeMessageVisibility(new ChangeMessageVisibilityRequest(SQS_QUEUE_NAME, longReceiptHandle, VisibilityTimeout));
            sqsMock.Verify(x => x.ChangeMessageVisibility(It.Is<ChangeMessageVisibilityRequest>(r =>
                r.VisibilityTimeout == VisibilityTimeout &&
                r.QueueUrl == SQS_QUEUE_NAME &&
                r.ReceiptHandle == Constants.HandleTail)), 
                Times.Once);
        }
#endif
    }
}