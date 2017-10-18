using System;
using Amazon.Runtime;

namespace Amazon.SQS.ExtendedClient
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;

    public abstract class AmazonSQSExtendedClientBase : IAmazonSQS
    {
        private readonly IAmazonSQS amazonSqsToBeExtended;

        protected AmazonSQSExtendedClientBase(IAmazonSQS sqsClient)
        {
            amazonSqsToBeExtended = sqsClient;
        }

        public void Dispose()
        {
            amazonSqsToBeExtended.Dispose();
        }

        public Task<Dictionary<String, String>> GetAttributesAsync(String queueUrl)
        {
            return amazonSqsToBeExtended.GetAttributesAsync(queueUrl);
        }

        public Task SetAttributesAsync(String queueUrl, Dictionary<String, String> attributes)
        {
            return amazonSqsToBeExtended.SetAttributesAsync(queueUrl, attributes);
        }

        public Task<String> AuthorizeS3ToSendMessageAsync(String queueUrl, String bucket)
        {
            return amazonSqsToBeExtended.AuthorizeS3ToSendMessageAsync(queueUrl, bucket);
        }

        public Task<AddPermissionResponse> AddPermissionAsync(string queueUrl, string label, List<string> awsAccountIds, List<string> actions, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.AddPermissionAsync(queueUrl, label, awsAccountIds, actions, cancellationToken);
        }

        public Task<AddPermissionResponse> AddPermissionAsync(AddPermissionRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.AddPermissionAsync(request, cancellationToken);
        }

        public Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(string queueUrl, string receiptHandle, int visibilityTimeout, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ChangeMessageVisibilityAsync(queueUrl, receiptHandle, visibilityTimeout, cancellationToken);
        }

        public Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(ChangeMessageVisibilityRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ChangeMessageVisibilityAsync(request, cancellationToken);
        }

        public Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(string queueUrl, List<ChangeMessageVisibilityBatchRequestEntry> entries, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ChangeMessageVisibilityBatchAsync(queueUrl, entries, cancellationToken);
        }

        public Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(ChangeMessageVisibilityBatchRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ChangeMessageVisibilityBatchAsync(request, cancellationToken);
        }

        public Task<CreateQueueResponse> CreateQueueAsync(string queueName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.CreateQueueAsync(queueName, cancellationToken);
        }

        public Task<CreateQueueResponse> CreateQueueAsync(CreateQueueRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.CreateQueueAsync(request, cancellationToken);
        }

        public virtual Task<DeleteMessageResponse> DeleteMessageAsync(string queueUrl, string receiptHandle, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.DeleteMessageAsync(queueUrl, receiptHandle, cancellationToken);
        }

        public virtual Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.DeleteMessageAsync(request, cancellationToken);
        }

        public virtual Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(string queueUrl, List<DeleteMessageBatchRequestEntry> entries, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.DeleteMessageBatchAsync(queueUrl, entries, cancellationToken);
        }

        public virtual Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(DeleteMessageBatchRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.DeleteMessageBatchAsync(request, cancellationToken);
        }

        public Task<DeleteQueueResponse> DeleteQueueAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.DeleteQueueAsync(queueUrl, cancellationToken);
        }

        public Task<DeleteQueueResponse> DeleteQueueAsync(DeleteQueueRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.DeleteQueueAsync(request, cancellationToken);
        }

        public Task<GetQueueAttributesResponse> GetQueueAttributesAsync(string queueUrl, List<string> attributeNames, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.GetQueueAttributesAsync(queueUrl, attributeNames, cancellationToken);
        }

        public Task<GetQueueAttributesResponse> GetQueueAttributesAsync(GetQueueAttributesRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.GetQueueAttributesAsync(request, cancellationToken);
        }

        public Task<GetQueueUrlResponse> GetQueueUrlAsync(string queueName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.GetQueueUrlAsync(queueName, cancellationToken);
        }

        public Task<GetQueueUrlResponse> GetQueueUrlAsync(GetQueueUrlRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.GetQueueUrlAsync(request, cancellationToken);
        }

        public Task<ListDeadLetterSourceQueuesResponse> ListDeadLetterSourceQueuesAsync(ListDeadLetterSourceQueuesRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ListDeadLetterSourceQueuesAsync(request, cancellationToken);
        }

        public Task<ListQueuesResponse> ListQueuesAsync(string queueNamePrefix, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ListQueuesAsync(queueNamePrefix, cancellationToken);
        }

        public Task<ListQueuesResponse> ListQueuesAsync(ListQueuesRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ListQueuesAsync(request, cancellationToken);
        }

        public Task<PurgeQueueResponse> PurgeQueueAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.PurgeQueueAsync(queueUrl, cancellationToken);
        }

        public Task<PurgeQueueResponse> PurgeQueueAsync(PurgeQueueRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.PurgeQueueAsync(request, cancellationToken);
        }

        public virtual Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ReceiveMessageAsync(queueUrl, cancellationToken);
        }

        public virtual Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ReceiveMessageAsync(request, cancellationToken);
        }

        public Task<RemovePermissionResponse> RemovePermissionAsync(string queueUrl, string label, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.RemovePermissionAsync(queueUrl, label, cancellationToken);
        }

        public Task<RemovePermissionResponse> RemovePermissionAsync(RemovePermissionRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.RemovePermissionAsync(request, cancellationToken);
        }

        public virtual Task<SendMessageResponse> SendMessageAsync(string queueUrl, string messageBody, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.SendMessageAsync(queueUrl, messageBody, cancellationToken);
        }

        public virtual Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.SendMessageAsync(request, cancellationToken);
        }

        public virtual Task<SendMessageBatchResponse> SendMessageBatchAsync(string queueUrl, List<SendMessageBatchRequestEntry> entries, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.SendMessageBatchAsync(queueUrl, entries, cancellationToken);
        }

        public virtual Task<SendMessageBatchResponse> SendMessageBatchAsync(SendMessageBatchRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.SendMessageBatchAsync(request, cancellationToken);
        }

        public Task<SetQueueAttributesResponse> SetQueueAttributesAsync(string queueUrl, Dictionary<string, string> attributes, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.SetQueueAttributesAsync(queueUrl, attributes, cancellationToken);
        }

        public Task<SetQueueAttributesResponse> SetQueueAttributesAsync(SetQueueAttributesRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.SetQueueAttributesAsync(request, cancellationToken);
        }

        public IClientConfig Config => amazonSqsToBeExtended.Config;
    }
}
