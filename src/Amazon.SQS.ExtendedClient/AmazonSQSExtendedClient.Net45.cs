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

    public partial class AmazonSQSExtendedClient
    {
#if NET45
        public override ChangeMessageVisibilityResponse ChangeMessageVisibility(string queueUrl, string receiptHandle, int visibilityTimeout)
        {
            return ChangeMessageVisibility(new ChangeMessageVisibilityRequest(queueUrl, receiptHandle, visibilityTimeout));
        }

        public override ChangeMessageVisibilityResponse ChangeMessageVisibility(ChangeMessageVisibilityRequest request)
        {
            request.ReceiptHandle = IsS3ReceiptHandle(request.ReceiptHandle)
                ? GetOriginalReceiptHandle(request.ReceiptHandle)
                : request.ReceiptHandle;
            return base.ChangeMessageVisibility(request);
        }

        public override ChangeMessageVisibilityBatchResponse ChangeMessageVisibilityBatch(string queueUrl, List<ChangeMessageVisibilityBatchRequestEntry> entries)
        {
            return ChangeMessageVisibilityBatch(new ChangeMessageVisibilityBatchRequest(queueUrl, entries));
        }

        public override ChangeMessageVisibilityBatchResponse ChangeMessageVisibilityBatch(ChangeMessageVisibilityBatchRequest request)
        {
            foreach (var entry in request.Entries)
            {
                entry.ReceiptHandle = IsS3ReceiptHandle(entry.ReceiptHandle)
                    ? GetOriginalReceiptHandle(entry.ReceiptHandle)
                    : entry.ReceiptHandle;
            }

            return base.ChangeMessageVisibilityBatch(request);
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
                if (!clientConfiguration.RetainS3Messages)
                {
                    DeleteMessagePayloadFromS3(deleteMessageRequest.ReceiptHandle);
                }

                deleteMessageRequest.ReceiptHandle = GetOriginalReceiptHandle(deleteMessageRequest.ReceiptHandle);
            }

            return base.DeleteMessage(deleteMessageRequest);
        }

        public override DeleteMessageResponse DeleteMessage(string queueUrl, string receiptHandle)
        {
            return DeleteMessage(new DeleteMessageRequest(queueUrl, receiptHandle));
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
                if (!clientConfiguration.RetainS3Messages)
                {
                    DeleteMessagePayloadFromS3(entry.ReceiptHandle);
                }

                entry.ReceiptHandle = GetOriginalReceiptHandle(entry.ReceiptHandle);
            }

            return base.DeleteMessageBatch(deleteMessageBatchRequest);
        }

        public override DeleteMessageBatchResponse DeleteMessageBatch(string queueUrl, List<DeleteMessageBatchRequestEntry> entries)
        {
            return DeleteMessageBatch(new DeleteMessageBatchRequest(queueUrl, entries));
        }

        private SendMessageBatchRequestEntry StoreMessageInS3(SendMessageBatchRequestEntry batchEntry)
        {
            CheckMessageAttributes(batchEntry.MessageAttributes);

            var s3Key = clientConfiguration.Is3KeyProvider.GenerateName();
            var messageContentStr = batchEntry.MessageBody;
            var messageContentSize = Encoding.UTF8.GetBytes(messageContentStr).LongCount();

            var messageAttributeValue = new MessageAttributeValue { DataType = "Number", StringValue = messageContentSize.ToString() };

            batchEntry.MessageAttributes.Add(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, messageAttributeValue);
            var s3Pointer = new MessageS3Pointer(clientConfiguration.S3BucketName, s3Key);

            StoreTextInS3(s3Key, messageContentStr);

            batchEntry.MessageBody = GetJsonFromS3Pointer(s3Pointer);

            return batchEntry;
        }

        private SendMessageRequest StoreMessageInS3(SendMessageRequest sendMessageRequest)
        {
            CheckMessageAttributes(sendMessageRequest.MessageAttributes);

            var s3Key = clientConfiguration.Is3KeyProvider.GenerateName();
            var messageContentStr = sendMessageRequest.MessageBody;
            var messageContentSize = Encoding.UTF8.GetBytes(messageContentStr).LongCount();

            var messageAttributeValue = new MessageAttributeValue { DataType = "Number", StringValue = messageContentSize.ToString() };

            sendMessageRequest.MessageAttributes.Add(SQSExtendedClientConstants.RESERVED_ATTRIBUTE_NAME, messageAttributeValue);
            var s3Pointer = new MessageS3Pointer(clientConfiguration.S3BucketName, s3Key);

            StoreTextInS3(s3Key, messageContentStr);

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

        private void StoreTextInS3(string s3Key, string messageContent)
        {
            try
            {
                clientConfiguration.S3.PutObject(new PutObjectRequest { BucketName = clientConfiguration.S3BucketName, Key = s3Key, ContentBody = messageContent });
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
#endif
    }
}