namespace Amazon.SQS.ExtendedClient.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using S3.Model;

    [TestFixture]
    public class When_ExtendedClient_Receives : ExtendedClientTestBase
    {
#if NET45
        [Test]
        public void Short_Message_It_Is_Not_Read_From_S3()
        {
            sqsMock.Setup(m => m.ReceiveMessage(It.IsAny<ReceiveMessageRequest>())).Returns(new ReceiveMessageResponse { Messages = Enumerable.Repeat(new Message(), 1).ToList() });
            client.ReceiveMessage(new ReceiveMessageRequest(SQS_QUEUE_NAME));
            sqsMock.Verify(s => s.ReceiveMessage(It.Is<ReceiveMessageRequest>(r => r.QueueUrl == SQS_QUEUE_NAME && r.MessageAttributeNames.Contains(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME))));
            s3Mock.Verify(s => s.GetObject(It.IsAny<GetObjectRequest>()), Times.Never);
        }
#endif

        [Test]
        public async Task Short_Message_Async_It_Is_Not_Read_From_S3()
        {
            sqsMock.Setup(m => m.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new ReceiveMessageResponse { Messages = Enumerable.Repeat(new Message(), 1).ToList() }));
            await client.ReceiveMessageAsync(new ReceiveMessageRequest(SQS_QUEUE_NAME));
            sqsMock.Verify(s => s.ReceiveMessageAsync(It.Is<ReceiveMessageRequest>(r => r.QueueUrl == SQS_QUEUE_NAME && r.MessageAttributeNames.Contains(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME)), It.IsAny<CancellationToken>()));
            s3Mock.Verify(s => s.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
#if NET45
        [Test]
        public void Long_Message_It_is_Read_From_S3()
        {
            var originalReceiptHadle = "_testrh_";
            var originalBody = "original";
            var messageKey = Guid.NewGuid().ToString("N");
            var pointer = new MessageS3Pointer(S3_BUCKET_NAME, messageKey);
            var message = new Message { Body = JsonConvert.SerializeObject(pointer), ReceiptHandle = originalReceiptHadle };
            message.MessageAttributes.Add(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, new MessageAttributeValue());

            sqsMock.Setup(m => m.ReceiveMessage(It.Is<ReceiveMessageRequest>(r => r.QueueUrl.Equals(SQS_QUEUE_NAME) && r.MessageAttributeNames.Contains(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME)))).Returns(new ReceiveMessageResponse { Messages = Enumerable.Repeat(message, 1).ToList() });
            s3Mock.Setup(m => m.GetObject(It.IsAny<GetObjectRequest>())).Returns(new GetObjectResponse { ResponseStream = new MemoryStream(Encoding.UTF8.GetBytes(originalBody)) });
            var response = client.ReceiveMessage(new ReceiveMessageRequest(SQS_QUEUE_NAME));

            s3Mock.Verify(s => s.GetObject(It.Is<GetObjectRequest>(r => r.BucketName == S3_BUCKET_NAME && r.Key == messageKey)), Times.Once);
            Assert.AreEqual(1, response.Messages.Count);
            Assert.AreEqual(originalBody, response.Messages[0].Body);
            Assert.AreEqual(GenerateReceiptHandle(S3_BUCKET_NAME, messageKey, originalReceiptHadle), response.Messages[0].ReceiptHandle);
        }
#endif

        [Test]
        public async Task Long_Message_Async_It_is_Read_From_S3()
        {
            var originalReceiptHadle = "_testrh_";
            var originalBody = "original";
            var messageKey = Guid.NewGuid().ToString("N");
            var pointer = new MessageS3Pointer(S3_BUCKET_NAME, messageKey);
            var message = new Message { Body = JsonConvert.SerializeObject(pointer), ReceiptHandle = originalReceiptHadle };
            message.MessageAttributes.Add(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, new MessageAttributeValue());

            sqsMock.Setup(m => m.ReceiveMessageAsync(It.Is<ReceiveMessageRequest>(r => r.MessageAttributeNames.Contains(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME)), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new ReceiveMessageResponse { Messages = Enumerable.Repeat(message, 1).ToList() }));
            s3Mock.Setup(m => m.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new GetObjectResponse { ResponseStream = new MemoryStream(Encoding.UTF8.GetBytes(originalBody)) }));
            var response = await client.ReceiveMessageAsync(new ReceiveMessageRequest(SQS_QUEUE_NAME));

            s3Mock.Verify(s => s.GetObjectAsync(It.Is<GetObjectRequest>(r => r.BucketName == S3_BUCKET_NAME && r.Key == messageKey), It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(1, response.Messages.Count);
            Assert.AreEqual(originalBody, response.Messages[0].Body);
            Assert.AreEqual(GenerateReceiptHandle(S3_BUCKET_NAME, messageKey, originalReceiptHadle), response.Messages[0].ReceiptHandle);
        }
    }
}