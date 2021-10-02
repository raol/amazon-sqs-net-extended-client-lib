using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using Moq;
using NUnit.Framework;

namespace Amazon.SQS.ExtendedClient.Tests
{
    public class When_Changing_Message_Visibility_Batch : ExtendedClientTestBase
    {
        private const int VisibilityTimeout = 100;

        [Test]
        public async Task Short_messages_Async()
        {
            var entries = new List<ChangeMessageVisibilityBatchRequestEntry>
            {
                new ChangeMessageVisibilityBatchRequestEntry("100", Constants.HandleTail)
                {
                    VisibilityTimeout = VisibilityTimeout
                }
            };
            await client.ChangeMessageVisibilityBatchAsync(new ChangeMessageVisibilityBatchRequest(SQS_QUEUE_NAME, entries));
            sqsMock.Verify(x => x.ChangeMessageVisibilityBatchAsync(It.Is<ChangeMessageVisibilityBatchRequest>(r =>
                        r.Entries[0].VisibilityTimeout == VisibilityTimeout &&
                        r.QueueUrl == SQS_QUEUE_NAME &&
                        r.Entries[0].ReceiptHandle == Constants.HandleTail),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Test]
        public async Task Long_messages_Async()
        {
            var s3Key = Guid.NewGuid().ToString("N");
            var handle = GenerateReceiptHandle(S3_BUCKET_NAME, s3Key, Constants.HandleTail);
            var entries = new List<ChangeMessageVisibilityBatchRequestEntry>
            {
                new ChangeMessageVisibilityBatchRequestEntry("100", handle)
                {
                    VisibilityTimeout = VisibilityTimeout
                }
            };
            await client.ChangeMessageVisibilityBatchAsync(new ChangeMessageVisibilityBatchRequest(SQS_QUEUE_NAME, entries));
            sqsMock.Verify(x => x.ChangeMessageVisibilityBatchAsync(It.Is<ChangeMessageVisibilityBatchRequest>(r =>
                        r.Entries[0].VisibilityTimeout == VisibilityTimeout &&
                        r.QueueUrl == SQS_QUEUE_NAME &&
                        r.Entries[0].ReceiptHandle == Constants.HandleTail),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

#if NET45

        [Test]
        public void Short_messages()
        {
            var entries = new List<ChangeMessageVisibilityBatchRequestEntry>
            {
                new ChangeMessageVisibilityBatchRequestEntry("100", Constants.HandleTail)
                {
                    VisibilityTimeout = VisibilityTimeout
                }
            };
            
            client.ChangeMessageVisibilityBatch(new ChangeMessageVisibilityBatchRequest(SQS_QUEUE_NAME, entries));
            sqsMock.Verify(x => x.ChangeMessageVisibilityBatch(It.Is<ChangeMessageVisibilityBatchRequest>(r =>
                        r.Entries[0].VisibilityTimeout == VisibilityTimeout &&
                        r.QueueUrl == SQS_QUEUE_NAME &&
                        r.Entries[0].ReceiptHandle == Constants.HandleTail)),
                Times.Once);
        }
        
        [Test]
        public void Long_messages()
        {
            var s3Key = Guid.NewGuid().ToString("N");
            var handle = GenerateReceiptHandle(S3_BUCKET_NAME, s3Key, Constants.HandleTail);
            var entries = new List<ChangeMessageVisibilityBatchRequestEntry>
            {
                new ChangeMessageVisibilityBatchRequestEntry("100", handle)
                {
                    VisibilityTimeout = VisibilityTimeout
                }
            };
            
            client.ChangeMessageVisibilityBatch(new ChangeMessageVisibilityBatchRequest(SQS_QUEUE_NAME, entries));
            sqsMock.Verify(x => x.ChangeMessageVisibilityBatch(It.Is<ChangeMessageVisibilityBatchRequest>(r =>
                        r.Entries[0].VisibilityTimeout == VisibilityTimeout &&
                        r.QueueUrl == SQS_QUEUE_NAME &&
                        r.Entries[0].ReceiptHandle == Constants.HandleTail)),
                Times.Once);
        }
#endif
    }
}