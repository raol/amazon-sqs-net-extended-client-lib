namespace Amazon.SQS.ExtendedClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;
    using Newtonsoft.Json;
    using Runtime;
    using S3.Model;

    public class AmazonSQSExtendedClient : AmazonSQSExtendedClientBase
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

        public override SendMessageResponse SendMessage(SendMessageRequest sendMessageRequest)
        {
            if (sendMessageRequest == null)
            {
                throw new AmazonClientException("sendMessageRequest cannot be null.");
            }

            if (string.IsNullOrEmpty(sendMessageRequest.MessageBody))
            {
                throw new AmazonClientException("MessageBody cannone be null or empty");
            }

            if (!clientConfiguration.IsLargePayloadSupportEnabled)
            {
                return base.SendMessage(sendMessageRequest);
            }

            if (clientConfiguration.AlwaysThroughS3 || IsLarge(sendMessageRequest))
            {
                sendMessageRequest = StoreMessageInS3(sendMessageRequest);
            }

            return base.SendMessage(sendMessageRequest);

        }

        public override SendMessageResponse SendMessage(string queueUrl, string messageBody)
        {
            return SendMessage(new SendMessageRequest(queueUrl, messageBody));
        }

        public async override Task<SendMessageResponse> SendMessageAsync(SendMessageRequest sendMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (sendMessageRequest == null)
            {
                throw new AmazonClientException("sendMessageRequest cannot be null.");
            }

            if (string.IsNullOrEmpty(sendMessageRequest.MessageBody))
            {
                throw new AmazonClientException("MessageBody cannone be null or empty");
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

        public override SendMessageBatchResponse SendMessageBatch(SendMessageBatchRequest sendMessageBatchRequest)
        {
            if (sendMessageBatchRequest == null)
            {
                throw new AmazonClientException("sendMessageBatch cannot be null");
            }

            if (!clientConfiguration.IsLargePayloadSupportEnabled)
            {
                return base.SendMessageBatch(sendMessageBatchRequest);
            }

            for (var i = 0; i < sendMessageBatchRequest.Entries.Count; i++)
            {
                if (clientConfiguration.AlwaysThroughS3 || IsLarge(sendMessageBatchRequest.Entries[i]))
                {
                    sendMessageBatchRequest.Entries[i] = StoreMessageInS3(sendMessageBatchRequest.Entries[i]);
                }
            }

            return base.SendMessageBatch(sendMessageBatchRequest);
        }

        public override SendMessageBatchResponse SendMessageBatch(string queueUrl, List<SendMessageBatchRequestEntry> entries)
        {
            return SendMessageBatch(new SendMessageBatchRequest(queueUrl, entries));
        }

        public async override Task<SendMessageBatchResponse> SendMessageBatchAsync(SendMessageBatchRequest sendMessageBatchRequest, CancellationToken cancellationToken = default(CancellationToken))
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

        public override ReceiveMessageResponse ReceiveMessage(ReceiveMessageRequest receiveMessageRequest)
        {
            if (receiveMessageRequest == null)
            {
                throw new AmazonClientException("receiveMessageRequest cannot be null");
            }

            if (!clientConfiguration.IsLargePayloadSupportEnabled)
            {
                return base.ReceiveMessage(receiveMessageRequest);
            }

            receiveMessageRequest.MessageAttributeNames.Add(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME);

            var receiveMessageResult = base.ReceiveMessage(receiveMessageRequest);
            foreach (var message in receiveMessageResult.Messages)
            {
                MessageAttributeValue largePayloadAttributeValue;
                if (message.MessageAttributes.TryGetValue(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, out largePayloadAttributeValue))
                {
                    var messageS3Pointer = ReadMessageS3PointerFromJson(message.Body);
                    var originalMessageBody = GetTextFromS3(messageS3Pointer.S3BucketName, messageS3Pointer.S3Key);
                    message.Body = originalMessageBody;
                    message.ReceiptHandle = EmbedS3PointerInReceiptHandle(message.ReceiptHandle, messageS3Pointer.S3BucketName, messageS3Pointer.S3Key);
                    message.MessageAttributes.Remove(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME);
                }
            }

            return receiveMessageResult;
        }

        public override ReceiveMessageResponse ReceiveMessage(string queueUrl)
        {
            return ReceiveMessage(new ReceiveMessageRequest(queueUrl));
        }

        public async override Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest receiveMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
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
                MessageAttributeValue largePayloadAttributeValue;
                if (message.MessageAttributes.TryGetValue(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, out largePayloadAttributeValue))
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

        public override DeleteMessageResponse DeleteMessage(DeleteMessageRequest deleteMessageRequest)
        {
            if (deleteMessageRequest == null)
            {
                throw new AmazonClientException("deleteMessageRequest cannot be null");
            }

            if (!clientConfiguration.IsLargePayloadSupportEnabled)
            {
                return base.DeleteMessage(deleteMessageRequest);
            }

            if (IsS3ReceiptHandle(deleteMessageRequest.ReceiptHandle))
            {
                DeleteMessagePayloadFromS3(deleteMessageRequest.ReceiptHandle);
                deleteMessageRequest.ReceiptHandle = GetOriginalReceiptHandle(deleteMessageRequest.ReceiptHandle);
            }

            return base.DeleteMessage(deleteMessageRequest);
        }

        public override DeleteMessageResponse DeleteMessage(string queueUrl, string receiptHandle)
        {
            return DeleteMessage(new DeleteMessageRequest(queueUrl, receiptHandle));
        }

        public async override Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequest deleteMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
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
                await DeleteMessagePayloadFromS3Async(deleteMessageRequest.ReceiptHandle, cancellationToken).ConfigureAwait(false);
                deleteMessageRequest.ReceiptHandle = GetOriginalReceiptHandle(deleteMessageRequest.ReceiptHandle);
            }

            return await base.DeleteMessageAsync(deleteMessageRequest, cancellationToken).ConfigureAwait(false);
        }

        public override Task<DeleteMessageResponse> DeleteMessageAsync(string queueUrl, string receiptHandle, CancellationToken cancellationToken = new CancellationToken())
        {
            return DeleteMessageAsync(new DeleteMessageRequest(queueUrl, receiptHandle), cancellationToken);
        }

        public override DeleteMessageBatchResponse DeleteMessageBatch(DeleteMessageBatchRequest deleteMessageBatchRequest)
        {
            if (deleteMessageBatchRequest == null)
            {
                throw new AmazonClientException("deleteMessageBatchRequest cannot be null");
            }

            if (!clientConfiguration.IsLargePayloadSupportEnabled)
            {
                return base.DeleteMessageBatch(deleteMessageBatchRequest);
            }

            foreach (var entry in deleteMessageBatchRequest.Entries.Where(entry => IsS3ReceiptHandle(entry.ReceiptHandle)))
            {
                DeleteMessagePayloadFromS3(entry.ReceiptHandle);
                entry.ReceiptHandle = GetOriginalReceiptHandle(entry.ReceiptHandle);
            }

            return base.DeleteMessageBatch(deleteMessageBatchRequest);
        }

        public override DeleteMessageBatchResponse DeleteMessageBatch(string queueUrl, List<DeleteMessageBatchRequestEntry> entries)
        {
            return DeleteMessageBatch(new DeleteMessageBatchRequest(queueUrl, entries));
        }

        public async override Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(DeleteMessageBatchRequest deleteMessageBatchRequest, CancellationToken cancellationToken = default(CancellationToken))
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
                await DeleteMessagePayloadFromS3Async(entry.ReceiptHandle, cancellationToken).ConfigureAwait(false);
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
            var contentSize = Encoding.UTF8.GetBytes(sendMessageRequest.MessageBody).LongLength;
            var attributesSize = GetAttributesSize(sendMessageRequest.MessageAttributes);

            return (contentSize + attributesSize > clientConfiguration.MessageSizeThreshold);
        }

        private bool IsLarge(SendMessageBatchRequestEntry batchEntry)
        {
            var contentSize = Encoding.UTF8.GetBytes(batchEntry.MessageBody).LongLength;
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

            MessageAttributeValue largePayloadAttributeValue;
            if (attributes.TryGetValue(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, out largePayloadAttributeValue))
            {
                var errorMessage = string.Format("Message attribute name {0}  is reserved for use by SQS extended client.", SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME);
                throw new AmazonClientException(errorMessage);
            }
        }

        private SendMessageBatchRequestEntry StoreMessageInS3(SendMessageBatchRequestEntry batchEntry)
        {
            CheckMessageAttributes(batchEntry.MessageAttributes);

            var s3Key = Guid.NewGuid().ToString("N");
            var messageContentStr = batchEntry.MessageBody;
            var messageContentSize = Encoding.UTF8.GetBytes(messageContentStr).LongLength;

            var messageAttributeValue = new MessageAttributeValue { DataType = "Number", StringValue = messageContentSize.ToString() };

            batchEntry.MessageAttributes.Add(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, messageAttributeValue);
            var s3Pointer = new MessageS3Pointer(clientConfiguration.S3BucketName, s3Key);

            StoreTextInS3(s3Key, messageContentStr);

            batchEntry.MessageBody = GetJsonFromS3Pointer(s3Pointer);

            return batchEntry;
        }

        private async Task<SendMessageBatchRequestEntry> StoreMessageInS3Async(SendMessageBatchRequestEntry batchEntry, CancellationToken cancellationToken= default(CancellationToken))
        {
            CheckMessageAttributes(batchEntry.MessageAttributes);

            var s3Key = Guid.NewGuid().ToString("N");
            var messageContentStr = batchEntry.MessageBody;
            var messageContentSize = Encoding.UTF8.GetBytes(messageContentStr).LongLength;

            var messageAttributeValue = new MessageAttributeValue { DataType = "Number", StringValue = messageContentSize.ToString() };

            batchEntry.MessageAttributes.Add(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, messageAttributeValue);
            var s3Pointer = new MessageS3Pointer(clientConfiguration.S3BucketName, s3Key);

            await StoreTextInS3Async(s3Key, messageContentStr, cancellationToken).ConfigureAwait(false);

            batchEntry.MessageBody = GetJsonFromS3Pointer(s3Pointer);

            return batchEntry;
        }

        private SendMessageRequest StoreMessageInS3(SendMessageRequest sendMessageRequest)
        {
            CheckMessageAttributes(sendMessageRequest.MessageAttributes);
            
            var s3Key = Guid.NewGuid().ToString("N");
            var messageContentStr = sendMessageRequest.MessageBody;
            var messageContentSize  = Encoding.UTF8.GetBytes(messageContentStr).LongLength;

            var messageAttributeValue = new MessageAttributeValue { DataType = "Number", StringValue = messageContentSize.ToString() };

            sendMessageRequest.MessageAttributes.Add(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, messageAttributeValue);
            var s3Pointer = new MessageS3Pointer(clientConfiguration.S3BucketName, s3Key);
            
            StoreTextInS3(s3Key, messageContentStr);
            
            sendMessageRequest.MessageBody = GetJsonFromS3Pointer(s3Pointer);

            return sendMessageRequest;
        }

        private async Task<SendMessageRequest> StoreMessageInS3Async(SendMessageRequest sendMessageRequest, CancellationToken cancellationToken = default (CancellationToken))
        {
            CheckMessageAttributes(sendMessageRequest.MessageAttributes);

            var s3Key = Guid.NewGuid().ToString("N");
            var messageContentStr = sendMessageRequest.MessageBody;
            var messageContentSize = Encoding.UTF8.GetBytes(messageContentStr).LongLength;

            var messageAttributeValue = new MessageAttributeValue { DataType = "Number", StringValue = messageContentSize.ToString() };

            sendMessageRequest.MessageAttributes.Add(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, messageAttributeValue);
            var s3Pointer = new MessageS3Pointer(clientConfiguration.S3BucketName, s3Key);

            await StoreTextInS3Async(s3Key, messageContentStr, cancellationToken).ConfigureAwait(false);

            sendMessageRequest.MessageBody = GetJsonFromS3Pointer(s3Pointer);

            return sendMessageRequest;
        }

        private void DeleteMessagePayloadFromS3(string receiptHandle)
        {
            var s3BucketName = GetValueFromReceiptHandleByMarker(receiptHandle, SQSExtendedClientConstants.S3_BUCKET_NAME_MARKER);
            var s3Key = GetValueFromReceiptHandleByMarker(receiptHandle, SQSExtendedClientConstants.S3_KEY_MARKER);

            try
            {
                clientConfiguration.S3.DeleteObject(s3BucketName, s3Key);
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

        private async Task DeleteMessagePayloadFromS3Async(string receiptHandle, CancellationToken cancellationToken = default (CancellationToken))
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

        private void StoreTextInS3(string s3Key, string messageContent)
        {
            try
            {
                clientConfiguration.S3.PutObject(new PutObjectRequest { BucketName = clientConfiguration.S3BucketName, Key = s3Key, ContentBody = messageContent });
            }
            catch (AmazonServiceException e)
            {
                throw new Exception("Failed to store the message content in an S3 object. SQS message was not sent.", e);
            }
            catch (AmazonClientException e)
            {
                throw new Exception("Failed to store the message content in an S3 object. SQS message was not sent.", e);
            }
        }

        private async Task StoreTextInS3Async(string s3Key, string messageContent, CancellationToken cancellationToken = default (CancellationToken))
        {
            try
            {
                await clientConfiguration.S3.PutObjectAsync(new PutObjectRequest { BucketName = clientConfiguration.S3BucketName, Key = s3Key, ContentBody = messageContent }, cancellationToken).ConfigureAwait(false);
            }
            catch (AmazonServiceException e)
            {
                throw new Exception("Failed to store the message content in an S3 object. SQS message was not sent.", e);
            }
            catch (AmazonClientException e)
            {
                throw new Exception("Failed to store the message content in an S3 object. SQS message was not sent.", e);
            }
        }

        private string GetTextFromS3(string s3BucketName, string s3Key)
        {
            var getObjectRequest = new GetObjectRequest { BucketName = s3BucketName, Key = s3Key };
            try
            {
                using (var getObjectResponse = clientConfiguration.S3.GetObject(getObjectRequest))
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

        private async Task<string> GetTextFromS3Async(string s3BucketName, string s3Key, CancellationToken cancellationToken = default (CancellationToken))
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