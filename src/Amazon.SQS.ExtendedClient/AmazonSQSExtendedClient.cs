namespace Amazon.SQS.ExtendedClient
{
    using Model;
    using Newtonsoft.Json;
    using Runtime;
    using S3.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class AmazonSQSExtendedClient : AmazonSQSExtendedClientBase
    {
        private readonly ExtendedClientConfiguration clientConfiguration;

        public AmazonSQSExtendedClient(IAmazonSQS sqsClient)
            : this(sqsClient, new ExtendedClientConfiguration())
        {

        }

        public AmazonSQSExtendedClient(IAmazonSQS sqsClient, ExtendedClientConfiguration configuration)
            : base(sqsClient)
        {
            clientConfiguration = configuration;
        }

        public override Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(string queueUrl, string receiptHandle, int visibilityTimeout,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return ChangeMessageVisibilityAsync(new ChangeMessageVisibilityRequest(queueUrl, receiptHandle, visibilityTimeout), cancellationToken);
        }

        public override Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(ChangeMessageVisibilityRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            request.ReceiptHandle = IsS3ReceiptHandle(request.ReceiptHandle)
                ? GetOriginalReceiptHandle(request.ReceiptHandle)
                : request.ReceiptHandle;

            return base.ChangeMessageVisibilityAsync(request, cancellationToken);
        }

        public override Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(string queueUrl, List<ChangeMessageVisibilityBatchRequestEntry> entries,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return ChangeMessageVisibilityBatchAsync(new ChangeMessageVisibilityBatchRequest(queueUrl, entries), cancellationToken);
        }

        public override Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(ChangeMessageVisibilityBatchRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var entry in request.Entries)
            {
                entry.ReceiptHandle = IsS3ReceiptHandle(entry.ReceiptHandle)
                    ? GetOriginalReceiptHandle(entry.ReceiptHandle)
                    : entry.ReceiptHandle;
            }

            return base.ChangeMessageVisibilityBatchAsync(request, cancellationToken);
        }

        public override async Task<SendMessageResponse> SendMessageAsync(SendMessageRequest sendMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (sendMessageRequest == null)
            {
                throw new AmazonClientException("sendMessageRequest cannot be null.");
            }

            if (string.IsNullOrEmpty(sendMessageRequest.MessageBody))
            {
                throw new AmazonClientException("MessageBody cannot be null or empty");
            }

            if (!clientConfiguration.IsLargePayloadSupportEnabled)
            {
                return await base.SendMessageAsync(sendMessageRequest, cancellationToken).ConfigureAwait(false);
            }

            if (clientConfiguration.AlwaysThroughS3 || IsLarge(sendMessageRequest))
            {
                sendMessageRequest = await StoreMessageInS3Async(sendMessageRequest, cancellationToken).ConfigureAwait(false);
            }

            return await base.SendMessageAsync(sendMessageRequest, cancellationToken).ConfigureAwait(false);
        }

        public override Task<SendMessageResponse> SendMessageAsync(string queueUrl, string messageBody, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendMessageAsync(new SendMessageRequest(queueUrl, messageBody), cancellationToken);
        }

        public override async Task<SendMessageBatchResponse> SendMessageBatchAsync(SendMessageBatchRequest sendMessageBatchRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (sendMessageBatchRequest == null)
            {
                throw new AmazonClientException("sendMessageBatch cannot be null");
            }

            if (!clientConfiguration.IsLargePayloadSupportEnabled)
            {
                return await base.SendMessageBatchAsync(sendMessageBatchRequest, cancellationToken).ConfigureAwait(false);
            }

            for (var i = 0; i < sendMessageBatchRequest.Entries.Count; i++)
            {
                if (clientConfiguration.AlwaysThroughS3 || IsLarge(sendMessageBatchRequest.Entries[i]))
                {
                    sendMessageBatchRequest.Entries[i] = await StoreMessageInS3Async(sendMessageBatchRequest.Entries[i], cancellationToken);
                }
            }

            return await base.SendMessageBatchAsync(sendMessageBatchRequest, cancellationToken);
        }

        public override Task<SendMessageBatchResponse> SendMessageBatchAsync(string queueUrl, List<SendMessageBatchRequestEntry> entries, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendMessageBatchAsync(new SendMessageBatchRequest(queueUrl, entries), cancellationToken);
        }

        public override async Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest receiveMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (receiveMessageRequest == null)
            {
                throw new AmazonClientException("receiveMessageRequest cannot be null");
            }

            if (!clientConfiguration.IsLargePayloadSupportEnabled)
            {
                return await base.ReceiveMessageAsync(receiveMessageRequest, cancellationToken).ConfigureAwait(false);
            }

            receiveMessageRequest.MessageAttributeNames.Add(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME);

            var receiveMessageResult = await base.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);
            foreach (var message in receiveMessageResult.Messages)
            {
                if (message.MessageAttributes.TryGetValue(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, out _))
                {
                    var messageS3Pointer = ReadMessageS3PointerFromJson(message.Body);
                    var originalMessageBody = await GetTextFromS3Async(messageS3Pointer.S3BucketName, messageS3Pointer.S3Key, cancellationToken).ConfigureAwait(false);
                    message.Body = originalMessageBody;
                    message.ReceiptHandle = EmbedS3PointerInReceiptHandle(message.ReceiptHandle, messageS3Pointer.S3BucketName, messageS3Pointer.S3Key);
                    message.MessageAttributes.Remove(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME);
                }
            }

            return receiveMessageResult;
        }

        public override Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ReceiveMessageAsync(new ReceiveMessageRequest(queueUrl), cancellationToken);
        }

        public override async Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequest deleteMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (deleteMessageRequest == null)
            {
                throw new AmazonClientException("deleteMessageRequest cannot be null");
            }

            if (!clientConfiguration.IsLargePayloadSupportEnabled)
            {
                return await base.DeleteMessageAsync(deleteMessageRequest, cancellationToken).ConfigureAwait(false);
            }

            if (IsS3ReceiptHandle(deleteMessageRequest.ReceiptHandle))
            {
                if (!clientConfiguration.RetainS3Messages)
                {
                    await DeleteMessagePayloadFromS3Async(deleteMessageRequest.ReceiptHandle, cancellationToken).ConfigureAwait(false);
                }

                deleteMessageRequest.ReceiptHandle = GetOriginalReceiptHandle(deleteMessageRequest.ReceiptHandle);
            }

            return await base.DeleteMessageAsync(deleteMessageRequest, cancellationToken).ConfigureAwait(false);
        }

        public override Task<DeleteMessageResponse> DeleteMessageAsync(string queueUrl, string receiptHandle, CancellationToken cancellationToken = new CancellationToken())
        {
            return DeleteMessageAsync(new DeleteMessageRequest(queueUrl, receiptHandle), cancellationToken);
        }

        public override async Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(DeleteMessageBatchRequest deleteMessageBatchRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (deleteMessageBatchRequest == null)
            {
                throw new AmazonClientException("deleteMessageBatchRequest cannot be null");
            }

            if (!clientConfiguration.IsLargePayloadSupportEnabled)
            {
                return await base.DeleteMessageBatchAsync(deleteMessageBatchRequest, cancellationToken).ConfigureAwait(false);
            }

            foreach (var entry in deleteMessageBatchRequest.Entries.Where(entry => IsS3ReceiptHandle(entry.ReceiptHandle)))
            {
                if (!clientConfiguration.RetainS3Messages)
                {
                    await DeleteMessagePayloadFromS3Async(entry.ReceiptHandle, cancellationToken).ConfigureAwait(false);
                }

                entry.ReceiptHandle = GetOriginalReceiptHandle(entry.ReceiptHandle);
            }

            return await base.DeleteMessageBatchAsync(deleteMessageBatchRequest, cancellationToken);
        }

        public override Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(string queueUrl, List<DeleteMessageBatchRequestEntry> entries, CancellationToken cancellationToken = default(CancellationToken))
        {
            return DeleteMessageBatchAsync(new DeleteMessageBatchRequest(queueUrl, entries), cancellationToken);
        }

        private string EmbedS3PointerInReceiptHandle(string receiptHandle, string s3BucketName, string s3Key)
        {
            return string.Concat(
                SQSExtendedClientConstants.S3_BUCKET_NAME_MARKER,
                s3BucketName,
                SQSExtendedClientConstants.S3_BUCKET_NAME_MARKER,
                SQSExtendedClientConstants.S3_KEY_MARKER,
                s3Key,
                SQSExtendedClientConstants.S3_KEY_MARKER,
                receiptHandle);
        }

        private bool IsS3ReceiptHandle(string receiptHandle)
        {
            return receiptHandle.Contains(SQSExtendedClientConstants.S3_BUCKET_NAME_MARKER) && receiptHandle.Contains(SQSExtendedClientConstants.S3_KEY_MARKER);
        }

        private string GetOriginalReceiptHandle(string receiptHandle)
        {
            var secondOccurence = receiptHandle.IndexOf(
                SQSExtendedClientConstants.S3_KEY_MARKER,
                receiptHandle.IndexOf(SQSExtendedClientConstants.S3_KEY_MARKER, StringComparison.Ordinal) + 1,
                StringComparison.Ordinal);
            return receiptHandle.Substring(secondOccurence + SQSExtendedClientConstants.S3_KEY_MARKER.Length);
        }

        private string GetValueFromReceiptHandleByMarker(string receiptHandle, string marker)
        {
            var firstOccurence = receiptHandle.IndexOf(marker, StringComparison.Ordinal);
            var secondOccurence = receiptHandle.IndexOf(marker, firstOccurence + 1, StringComparison.Ordinal);
            return receiptHandle.Substring(firstOccurence + marker.Length, secondOccurence - (firstOccurence + marker.Length));
        }

        private bool IsLarge(SendMessageRequest sendMessageRequest)
        {
            var contentSize = Encoding.UTF8.GetBytes(sendMessageRequest.MessageBody).LongCount();
            var attributesSize = GetAttributesSize(sendMessageRequest.MessageAttributes);

            return (contentSize + attributesSize > clientConfiguration.MessageSizeThreshold);
        }

        private bool IsLarge(SendMessageBatchRequestEntry batchEntry)
        {
            var contentSize = Encoding.UTF8.GetBytes(batchEntry.MessageBody).LongCount();
            var attributesSize = GetAttributesSize(batchEntry.MessageAttributes);

            return (contentSize + attributesSize > clientConfiguration.MessageSizeThreshold);
        }

        private int GetAttributesSize(Dictionary<string, MessageAttributeValue> attributes)
        {
            var attributesSize = 0;
            foreach (var messageAttributeValue in attributes)
            {
                attributesSize += Encoding.UTF8.GetByteCount(messageAttributeValue.Key);
                if (!string.IsNullOrEmpty(messageAttributeValue.Value.DataType))
                {
                    attributesSize += Encoding.UTF8.GetByteCount(messageAttributeValue.Value.DataType);
                }

                var stringValue = messageAttributeValue.Value.StringValue;
                if (!string.IsNullOrEmpty(stringValue))
                {
                    attributesSize += Encoding.UTF8.GetByteCount(stringValue);
                }

                var binaryValue = messageAttributeValue.Value.BinaryValue;
                if (binaryValue != null)
                {
                    attributesSize += binaryValue.ToArray().Length;
                }
            }

            return attributesSize;
        }

        private void CheckMessageAttributes(Dictionary<string, MessageAttributeValue> attributes)
        {
            var attributesSize = GetAttributesSize(attributes);

            if (attributesSize > clientConfiguration.MessageSizeThreshold)
            {
                var errorMessage = string.Format(
                    "Total size of Message attributes is {0} bytes which is larger than the threshold of {1}  Bytes. " +
                    "Consider including the payload in the message body instead of message attributes.",
                    attributesSize,
                    clientConfiguration.MessageSizeThreshold);
                throw new AmazonClientException(errorMessage);
            }

            if (attributes.Count > SQSExtendedClientConstants.MAX_ALLOWED_ATTRIBUTES)
            {
                var errorMessage = string.Format("Number of message attributes [{0}] ] exceeds the maximum allowed for large-payload messages [{1}]",
                    attributes.Count,
                    SQSExtendedClientConstants.MAX_ALLOWED_ATTRIBUTES);
                throw new AmazonClientException(errorMessage);
            }

            if (attributes.TryGetValue(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, out _))
            {
                var errorMessage = string.Format("Message attribute name {0}  is reserved for use by SQS extended client.", SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME);
                throw new AmazonClientException(errorMessage);
            }
        }

        private async Task<SendMessageBatchRequestEntry> StoreMessageInS3Async(SendMessageBatchRequestEntry batchEntry, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckMessageAttributes(batchEntry.MessageAttributes);

            var s3Key = clientConfiguration.Is3KeyProvider.GenerateName();
            var messageContentStr = batchEntry.MessageBody;
            var messageContentSize = Encoding.UTF8.GetBytes(messageContentStr).LongCount();

            var messageAttributeValue = new MessageAttributeValue { DataType = "Number", StringValue = messageContentSize.ToString() };

            batchEntry.MessageAttributes.Add(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, messageAttributeValue);
            var s3Pointer = new MessageS3Pointer(clientConfiguration.S3BucketName, s3Key);

            await StoreTextInS3Async(s3Key, messageContentStr, cancellationToken).ConfigureAwait(false);

            batchEntry.MessageBody = GetJsonFromS3Pointer(s3Pointer);

            return batchEntry;
        }

        private async Task<SendMessageRequest> StoreMessageInS3Async(SendMessageRequest sendMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckMessageAttributes(sendMessageRequest.MessageAttributes);

            var s3Key = clientConfiguration.Is3KeyProvider.GenerateName();
            var messageContentStr = sendMessageRequest.MessageBody;
            var messageContentSize = Encoding.UTF8.GetBytes(messageContentStr).LongCount();

            var messageAttributeValue = new MessageAttributeValue { DataType = "Number", StringValue = messageContentSize.ToString() };

            sendMessageRequest.MessageAttributes.Add(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, messageAttributeValue);
            var s3Pointer = new MessageS3Pointer(clientConfiguration.S3BucketName, s3Key);

            await StoreTextInS3Async(s3Key, messageContentStr, cancellationToken).ConfigureAwait(false);

            sendMessageRequest.MessageBody = GetJsonFromS3Pointer(s3Pointer);

            return sendMessageRequest;
        }

        private async Task DeleteMessagePayloadFromS3Async(string receiptHandle, CancellationToken cancellationToken = default(CancellationToken))
        {
            var s3BucketName = GetValueFromReceiptHandleByMarker(receiptHandle, SQSExtendedClientConstants.S3_BUCKET_NAME_MARKER);
            var s3Key = GetValueFromReceiptHandleByMarker(receiptHandle, SQSExtendedClientConstants.S3_KEY_MARKER);

            try
            {
                await clientConfiguration.S3.DeleteObjectAsync(s3BucketName, s3Key, cancellationToken).ConfigureAwait(false);
            }
            catch (AmazonServiceException e)
            {
                throw new AmazonClientException("Failed to delete the S3 object which contains the SQS message payload. SQS message was not deleted.", e);
            }
            catch (AmazonClientException e)
            {
                throw new AmazonClientException("Failed to delete the S3 object which contains the SQS message payload. SQS message was not deleted.", e);
            }
        }

        private async Task StoreTextInS3Async(string s3Key, string messageContent, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = clientConfiguration.S3BucketName,
                    Key = s3Key,
                    ContentBody = messageContent,
                    CannedACL = clientConfiguration.S3CannedACL
                };

                await clientConfiguration.S3.PutObjectAsync(putObjectRequest, cancellationToken).ConfigureAwait(false);
            }
            catch (AmazonServiceException e)
            {
                throw new AmazonClientException("Failed to store the message content in an S3 object. SQS message was not sent.", e);
            }
            catch (AmazonClientException e)
            {
                throw new AmazonClientException("Failed to store the message content in an S3 object. SQS message was not sent.", e);
            }
        }

        private async Task<string> GetTextFromS3Async(string s3BucketName, string s3Key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var getObjectRequest = new GetObjectRequest { BucketName = s3BucketName, Key = s3Key };
            try
            {
                using (var getObjectResponse = await clientConfiguration.S3.GetObjectAsync(getObjectRequest, cancellationToken).ConfigureAwait(false))
                {
                    var streamReader = new StreamReader(getObjectResponse.ResponseStream);
                    var text = streamReader.ReadToEnd();
                    return text;
                }
            }
            catch (AmazonServiceException e)
            {
                throw new AmazonClientException("Failed to get the S3 object which contains the message payload. Message was not received.", e);
            }
            catch (AmazonClientException e)
            {
                throw new AmazonClientException("Failed to get the S3 object which contains the message payload. Message was not received.", e);
            }
        }

        private string GetJsonFromS3Pointer(MessageS3Pointer s3Pointer)
        {
            try
            {
                return JsonConvert.SerializeObject(s3Pointer);
            }
            catch (Exception e)
            {
                throw new AmazonClientException("Failed to convert S3 object pointer to text. Message was not sent.", e);
            }
        }

        private MessageS3Pointer ReadMessageS3PointerFromJson(string messageBody)
        {
            try
            {
                return JsonConvert.DeserializeObject<MessageS3Pointer>(messageBody);
            }
            catch (Exception e)
            {
                throw new AmazonClientException("Failed to read the S3 object pointer from an SQS message. Message was not received.", e);
            }
        }
    }
}