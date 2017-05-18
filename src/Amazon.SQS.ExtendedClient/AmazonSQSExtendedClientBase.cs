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

        public Task<Dictionary<string, string>> GetAttributesAsync(string queueUrl)
        {
            return amazonSqsToBeExtended.GetAttributesAsync(queueUrl);
        }

        public Task SetAttributesAsync(string queueUrl, Dictionary<string, string> attributes)
        {
            return amazonSqsToBeExtended.SetAttributesAsync(queueUrl, attributes);
        }

        public IClientConfig Config => amazonSqsToBeExtended.Config;


        public async Task<string> AuthorizeS3ToSendMessageAsync(string queueUrl, string bucket)
        {
            return await amazonSqsToBeExtended.AuthorizeS3ToSendMessageAsync(queueUrl, bucket);
        }

        public async Task<AddPermissionResponse> AddPermissionAsync(string queueUrl, string label, List<string> awsAccountIds, List<string> actions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.AddPermissionAsync(queueUrl, label, awsAccountIds, actions, cancellationToken);
        }

        public async Task<AddPermissionResponse> AddPermissionAsync(AddPermissionRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.AddPermissionAsync(request, cancellationToken);
        }

        public async Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(string queueUrl, string receiptHandle, int visibilityTimeout,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.ChangeMessageVisibilityAsync(queueUrl, receiptHandle, visibilityTimeout, cancellationToken);
        }

        public async Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(ChangeMessageVisibilityRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.ChangeMessageVisibilityAsync(request, cancellationToken);
        }

        public async Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(string queueUrl, List<ChangeMessageVisibilityBatchRequestEntry> entries,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.ChangeMessageVisibilityBatchAsync(queueUrl, entries, cancellationToken);
        }

        public async Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(ChangeMessageVisibilityBatchRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.ChangeMessageVisibilityBatchAsync(request, cancellationToken);
        }

        public async Task<CreateQueueResponse> CreateQueueAsync(string queueName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.CreateQueueAsync(queueName, cancellationToken);
        }

        public async Task<CreateQueueResponse> CreateQueueAsync(CreateQueueRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.CreateQueueAsync(request, cancellationToken);
        }

        public virtual async Task<DeleteMessageResponse> DeleteMessageAsync(string queueUrl, string receiptHandle,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.DeleteMessageAsync(queueUrl, receiptHandle, cancellationToken);
        }

        public virtual async Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.DeleteMessageAsync(request, cancellationToken);
        }

        public virtual async Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(string queueUrl, List<DeleteMessageBatchRequestEntry> entries,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.DeleteMessageBatchAsync(queueUrl, entries, cancellationToken);
        }

        public virtual async Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(DeleteMessageBatchRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.DeleteMessageBatchAsync(request, cancellationToken);
        }

        public async Task<DeleteQueueResponse> DeleteQueueAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.DeleteQueueAsync(queueUrl, cancellationToken);
        }

        public async Task<DeleteQueueResponse> DeleteQueueAsync(DeleteQueueRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.DeleteQueueAsync(request, cancellationToken);
        }

        public async Task<GetQueueAttributesResponse> GetQueueAttributesAsync(string queueUrl, List<string> attributeNames,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.GetQueueAttributesAsync(queueUrl, attributeNames, cancellationToken);
        }

        public async Task<GetQueueAttributesResponse> GetQueueAttributesAsync(GetQueueAttributesRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.GetQueueAttributesAsync(request, cancellationToken);
        }

        public async Task<GetQueueUrlResponse> GetQueueUrlAsync(string queueName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.GetQueueUrlAsync(queueName, cancellationToken);
        }

        public async Task<GetQueueUrlResponse> GetQueueUrlAsync(GetQueueUrlRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.GetQueueUrlAsync(request, cancellationToken);
        }

        public async Task<ListDeadLetterSourceQueuesResponse> ListDeadLetterSourceQueuesAsync(ListDeadLetterSourceQueuesRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.ListDeadLetterSourceQueuesAsync(request, cancellationToken);
        }

        public async Task<ListQueuesResponse> ListQueuesAsync(string queueNamePrefix, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.ListQueuesAsync(queueNamePrefix, cancellationToken);
        }

        public async Task<ListQueuesResponse> ListQueuesAsync(ListQueuesRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.ListQueuesAsync(request, cancellationToken);
        }

        public async Task<PurgeQueueResponse> PurgeQueueAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.PurgeQueueAsync(queueUrl, cancellationToken);
        }

        public async Task<PurgeQueueResponse> PurgeQueueAsync(PurgeQueueRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.PurgeQueueAsync(request, cancellationToken);
        }

        public virtual async Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.ReceiveMessageAsync(queueUrl, cancellationToken);
        }

        public virtual async Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.ReceiveMessageAsync(request, cancellationToken);
        }

        public async Task<RemovePermissionResponse> RemovePermissionAsync(string queueUrl, string label,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.RemovePermissionAsync(queueUrl, label, cancellationToken);
        }

        public async Task<RemovePermissionResponse> RemovePermissionAsync(RemovePermissionRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.RemovePermissionAsync(request, cancellationToken);
        }

        public virtual async Task<SendMessageResponse> SendMessageAsync(string queueUrl, string messageBody,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.SendMessageAsync(queueUrl, messageBody, cancellationToken);
        }

        public virtual async Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.SendMessageAsync(request, cancellationToken);
        }

        public virtual async Task<SendMessageBatchResponse> SendMessageBatchAsync(string queueUrl, List<SendMessageBatchRequestEntry> entries,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.SendMessageBatchAsync(queueUrl, entries, cancellationToken);
        }

        public virtual async Task<SendMessageBatchResponse> SendMessageBatchAsync(SendMessageBatchRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.SendMessageBatchAsync(request, cancellationToken);
        }

        public async Task<SetQueueAttributesResponse> SetQueueAttributesAsync(string queueUrl, Dictionary<string, string> attributes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.SetQueueAttributesAsync(queueUrl, attributes, cancellationToken);
        }

        public async Task<SetQueueAttributesResponse> SetQueueAttributesAsync(SetQueueAttributesRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await amazonSqsToBeExtended.SetQueueAttributesAsync(request, cancellationToken);
        }
    }
}
