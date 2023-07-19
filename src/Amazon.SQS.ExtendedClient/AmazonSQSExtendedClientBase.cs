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

#if NET45
        public Dictionary<string, string> GetAttributes(string queueUrl)
        {
            return amazonSqsToBeExtended.GetAttributes(queueUrl);
        }
#endif

        public Task<Dictionary<String, String>> GetAttributesAsync(String queueUrl)
        {
            return amazonSqsToBeExtended.GetAttributesAsync(queueUrl);
        }

#if NET45
        public void SetAttributes(string queueUrl, Dictionary<string, string> attributes)
        {
            amazonSqsToBeExtended.SetAttributes(queueUrl, attributes);
        }
#endif

        public Task SetAttributesAsync(String queueUrl, Dictionary<String, String> attributes)
        {
            return amazonSqsToBeExtended.SetAttributesAsync(queueUrl, attributes);
        }

#if NET45
        public string AuthorizeS3ToSendMessage(string queueUrl, string bucket)
        {
            return amazonSqsToBeExtended.AuthorizeS3ToSendMessage(queueUrl, bucket);
        }
#endif

        public Task<String> AuthorizeS3ToSendMessageAsync(String queueUrl, String bucket)
        {
            return amazonSqsToBeExtended.AuthorizeS3ToSendMessageAsync(queueUrl, bucket);
        }

#if NET45
        public AddPermissionResponse AddPermission(string queueUrl, string label, List<string> awsAccountIds, List<string> actions)
        {
            return amazonSqsToBeExtended.AddPermission(queueUrl, label, awsAccountIds, actions);
        }

        public AddPermissionResponse AddPermission(AddPermissionRequest request)
        {
            return amazonSqsToBeExtended.AddPermission(request);
        }
#endif

        public Task<AddPermissionResponse> AddPermissionAsync(string queueUrl, string label, List<string> awsAccountIds, List<string> actions, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.AddPermissionAsync(queueUrl, label, awsAccountIds, actions, cancellationToken);
        }

        public Task<AddPermissionResponse> AddPermissionAsync(AddPermissionRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.AddPermissionAsync(request, cancellationToken);
        }

#if NET45
        public CancelMessageMoveTaskResponse CancelMessageMoveTask(CancelMessageMoveTaskRequest request)
        {
            return amazonSqsToBeExtended.CancelMessageMoveTask(request);
        }
#endif

        public Task<CancelMessageMoveTaskResponse> CancelMessageMoveTaskAsync(CancelMessageMoveTaskRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return amazonSqsToBeExtended.CancelMessageMoveTaskAsync(request, cancellationToken);
        }

#if NET45
        public virtual ChangeMessageVisibilityResponse ChangeMessageVisibility(string queueUrl, string receiptHandle, int visibilityTimeout)
        {
            return amazonSqsToBeExtended.ChangeMessageVisibility(queueUrl, receiptHandle, visibilityTimeout);
        }

        public virtual ChangeMessageVisibilityResponse ChangeMessageVisibility(ChangeMessageVisibilityRequest request)
        {
            return amazonSqsToBeExtended.ChangeMessageVisibility(request);
        }
#endif

        public virtual Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(string queueUrl, string receiptHandle, int visibilityTimeout, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ChangeMessageVisibilityAsync(queueUrl, receiptHandle, visibilityTimeout, cancellationToken);
        }

        public virtual Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(ChangeMessageVisibilityRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ChangeMessageVisibilityAsync(request, cancellationToken);
        }

#if NET45
        public virtual ChangeMessageVisibilityBatchResponse ChangeMessageVisibilityBatch(string queueUrl, List<ChangeMessageVisibilityBatchRequestEntry> entries)
        {
            return amazonSqsToBeExtended.ChangeMessageVisibilityBatch(queueUrl, entries);
        }

        public virtual ChangeMessageVisibilityBatchResponse ChangeMessageVisibilityBatch(ChangeMessageVisibilityBatchRequest request)
        {
            return amazonSqsToBeExtended.ChangeMessageVisibilityBatch(request);
        }
#endif

        public virtual Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(string queueUrl, List<ChangeMessageVisibilityBatchRequestEntry> entries, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ChangeMessageVisibilityBatchAsync(queueUrl, entries, cancellationToken);
        }

        public virtual Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(ChangeMessageVisibilityBatchRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ChangeMessageVisibilityBatchAsync(request, cancellationToken);
        }

#if NET45
        public CreateQueueResponse CreateQueue(string queueName)
        {
            return amazonSqsToBeExtended.CreateQueue(queueName);
        }

        public CreateQueueResponse CreateQueue(CreateQueueRequest request)
        {
            return amazonSqsToBeExtended.CreateQueue(request);
        }
#endif

        public Task<CreateQueueResponse> CreateQueueAsync(string queueName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.CreateQueueAsync(queueName, cancellationToken);
        }

        public Task<CreateQueueResponse> CreateQueueAsync(CreateQueueRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.CreateQueueAsync(request, cancellationToken);
        }

#if NET45
        public virtual DeleteMessageResponse DeleteMessage(string queueUrl, string receiptHandle)
        {
            return amazonSqsToBeExtended.DeleteMessage(queueUrl, receiptHandle);
        }

        public virtual DeleteMessageResponse DeleteMessage(DeleteMessageRequest request)
        {
            return amazonSqsToBeExtended.DeleteMessage(request);
        }
#endif
        public virtual Task<DeleteMessageResponse> DeleteMessageAsync(string queueUrl, string receiptHandle, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.DeleteMessageAsync(queueUrl, receiptHandle, cancellationToken);
        }

        public virtual Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.DeleteMessageAsync(request, cancellationToken);
        }

#if NET45
        public virtual DeleteMessageBatchResponse DeleteMessageBatch(string queueUrl, List<DeleteMessageBatchRequestEntry> entries)
        {
            return amazonSqsToBeExtended.DeleteMessageBatch(queueUrl, entries);
        }

        public virtual DeleteMessageBatchResponse DeleteMessageBatch(DeleteMessageBatchRequest request)
        {
            return amazonSqsToBeExtended.DeleteMessageBatch(request);
        }
#endif

        public virtual Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(string queueUrl, List<DeleteMessageBatchRequestEntry> entries, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.DeleteMessageBatchAsync(queueUrl, entries, cancellationToken);
        }

        public virtual Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(DeleteMessageBatchRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.DeleteMessageBatchAsync(request, cancellationToken);
        }

#if NET45
        public DeleteQueueResponse DeleteQueue(string queueUrl)
        {
            return amazonSqsToBeExtended.DeleteQueue(queueUrl);
        }

        public DeleteQueueResponse DeleteQueue(DeleteQueueRequest request)
        {
            return amazonSqsToBeExtended.DeleteQueue(request);
        }
#endif

        public Task<DeleteQueueResponse> DeleteQueueAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.DeleteQueueAsync(queueUrl, cancellationToken);
        }

        public Task<DeleteQueueResponse> DeleteQueueAsync(DeleteQueueRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.DeleteQueueAsync(request, cancellationToken);
        }

#if NET45
        public GetQueueAttributesResponse GetQueueAttributes(string queueUrl, List<string> attributeNames)
        {
            return amazonSqsToBeExtended.GetQueueAttributes(queueUrl, attributeNames);
        }

        public GetQueueAttributesResponse GetQueueAttributes(GetQueueAttributesRequest request)
        {
            return amazonSqsToBeExtended.GetQueueAttributes(request);
        }
#endif

        public Task<GetQueueAttributesResponse> GetQueueAttributesAsync(string queueUrl, List<string> attributeNames, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.GetQueueAttributesAsync(queueUrl, attributeNames, cancellationToken);
        }

        public Task<GetQueueAttributesResponse> GetQueueAttributesAsync(GetQueueAttributesRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.GetQueueAttributesAsync(request, cancellationToken);
        }

#if NET45
        public GetQueueUrlResponse GetQueueUrl(string queueName)
        {
            return amazonSqsToBeExtended.GetQueueUrl(queueName);
        }

        public GetQueueUrlResponse GetQueueUrl(GetQueueUrlRequest request)
        {
            return amazonSqsToBeExtended.GetQueueUrl(request);
        }
#endif

        public Task<GetQueueUrlResponse> GetQueueUrlAsync(string queueName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.GetQueueUrlAsync(queueName, cancellationToken);
        }

        public Task<GetQueueUrlResponse> GetQueueUrlAsync(GetQueueUrlRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.GetQueueUrlAsync(request, cancellationToken);
        }

#if NET45
        public ListDeadLetterSourceQueuesResponse ListDeadLetterSourceQueues(ListDeadLetterSourceQueuesRequest request)
        {
            return amazonSqsToBeExtended.ListDeadLetterSourceQueues(request);
        }
#endif

        public Task<ListDeadLetterSourceQueuesResponse> ListDeadLetterSourceQueuesAsync(ListDeadLetterSourceQueuesRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ListDeadLetterSourceQueuesAsync(request, cancellationToken);
        }
#if NET45
        public ListMessageMoveTasksResponse ListMessageMoveTasks(ListMessageMoveTasksRequest request)
        {
            return amazonSqsToBeExtended.ListMessageMoveTasks(request);
        }
#endif
        public Task<ListMessageMoveTasksResponse> ListMessageMoveTasksAsync(ListMessageMoveTasksRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return amazonSqsToBeExtended.ListMessageMoveTasksAsync(request, cancellationToken);
        }

#if NET45
        public ListQueuesResponse ListQueues(string queueNamePrefix)
        {
            return amazonSqsToBeExtended.ListQueues(queueNamePrefix);
        }

        public ListQueuesResponse ListQueues(ListQueuesRequest request)
        {
            return amazonSqsToBeExtended.ListQueues(request);
        }
#endif

        public Task<ListQueuesResponse> ListQueuesAsync(string queueNamePrefix, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ListQueuesAsync(queueNamePrefix, cancellationToken);
        }

        public Task<ListQueuesResponse> ListQueuesAsync(ListQueuesRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ListQueuesAsync(request, cancellationToken);
        }

#if NET45
        public ListQueueTagsResponse ListQueueTags(ListQueueTagsRequest request)
        {
            return amazonSqsToBeExtended.ListQueueTags(request);
        }
#endif

        public Task<ListQueueTagsResponse> ListQueueTagsAsync(ListQueueTagsRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return amazonSqsToBeExtended.ListQueueTagsAsync(request, cancellationToken);
        }

#if NET45
        public PurgeQueueResponse PurgeQueue(string queueUrl)
        {
            return amazonSqsToBeExtended.PurgeQueue(queueUrl);
        }

        public PurgeQueueResponse PurgeQueue(PurgeQueueRequest request)
        {
            return amazonSqsToBeExtended.PurgeQueue(request);
        }
#endif

        public Task<PurgeQueueResponse> PurgeQueueAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.PurgeQueueAsync(queueUrl, cancellationToken);
        }

        public Task<PurgeQueueResponse> PurgeQueueAsync(PurgeQueueRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.PurgeQueueAsync(request, cancellationToken);
        }

#if NET45
        public virtual ReceiveMessageResponse ReceiveMessage(string queueUrl)
        {
            return amazonSqsToBeExtended.ReceiveMessage(queueUrl);
        }

        public virtual ReceiveMessageResponse ReceiveMessage(ReceiveMessageRequest request)
        {
            return amazonSqsToBeExtended.ReceiveMessage(request);
        }
#endif

        public virtual Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ReceiveMessageAsync(queueUrl, cancellationToken);
        }

        public virtual Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.ReceiveMessageAsync(request, cancellationToken);
        }

#if NET45
        public RemovePermissionResponse RemovePermission(string queueUrl, string label)
        {
            return amazonSqsToBeExtended.RemovePermission(queueUrl, label);
        }

        public RemovePermissionResponse RemovePermission(RemovePermissionRequest request)
        {
            return amazonSqsToBeExtended.RemovePermission(request);
        }
#endif

        public Task<RemovePermissionResponse> RemovePermissionAsync(string queueUrl, string label, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.RemovePermissionAsync(queueUrl, label, cancellationToken);
        }

        public Task<RemovePermissionResponse> RemovePermissionAsync(RemovePermissionRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.RemovePermissionAsync(request, cancellationToken);
        }

#if NET45
        public virtual SendMessageResponse SendMessage(string queueUrl, string messageBody)
        {
            return amazonSqsToBeExtended.SendMessage(queueUrl, messageBody);
        }

        public virtual SendMessageResponse SendMessage(SendMessageRequest request)
        {
            return amazonSqsToBeExtended.SendMessage(request);
        }
#endif

        public virtual Task<SendMessageResponse> SendMessageAsync(string queueUrl, string messageBody, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.SendMessageAsync(queueUrl, messageBody, cancellationToken);
        }

        public virtual Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.SendMessageAsync(request, cancellationToken);
        }

#if NET45
        public virtual SendMessageBatchResponse SendMessageBatch(string queueUrl, List<SendMessageBatchRequestEntry> entries)
        {
            return amazonSqsToBeExtended.SendMessageBatch(queueUrl, entries);
        }

        public virtual SendMessageBatchResponse SendMessageBatch(SendMessageBatchRequest request)
        {
            return amazonSqsToBeExtended.SendMessageBatch(request);
        }
#endif

        public virtual Task<SendMessageBatchResponse> SendMessageBatchAsync(string queueUrl, List<SendMessageBatchRequestEntry> entries, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.SendMessageBatchAsync(queueUrl, entries, cancellationToken);
        }

        public virtual Task<SendMessageBatchResponse> SendMessageBatchAsync(SendMessageBatchRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.SendMessageBatchAsync(request, cancellationToken);
        }

#if NET45
        public SetQueueAttributesResponse SetQueueAttributes(string queueUrl, Dictionary<string, string> attributes)
        {
            return amazonSqsToBeExtended.SetQueueAttributes(queueUrl, attributes);
        }

        public SetQueueAttributesResponse SetQueueAttributes(SetQueueAttributesRequest request)
        {
            return amazonSqsToBeExtended.SetQueueAttributes(request);
        }
#endif

        public Task<SetQueueAttributesResponse> SetQueueAttributesAsync(string queueUrl, Dictionary<string, string> attributes, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.SetQueueAttributesAsync(queueUrl, attributes, cancellationToken);
        }

        public Task<SetQueueAttributesResponse> SetQueueAttributesAsync(SetQueueAttributesRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return amazonSqsToBeExtended.SetQueueAttributesAsync(request, cancellationToken);
        }

#if NET45
        public StartMessageMoveTaskResponse StartMessageMoveTask(StartMessageMoveTaskRequest request)
        {
            return amazonSqsToBeExtended.StartMessageMoveTask(request);
        }
#endif

        public Task<StartMessageMoveTaskResponse> StartMessageMoveTaskAsync(StartMessageMoveTaskRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return amazonSqsToBeExtended.StartMessageMoveTaskAsync(request, cancellationToken);
        }

#if NET45
        public TagQueueResponse TagQueue(TagQueueRequest request)
        {
            return amazonSqsToBeExtended.TagQueue(request);
        }
#endif

        public Task<TagQueueResponse> TagQueueAsync(TagQueueRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return amazonSqsToBeExtended.TagQueueAsync(request, cancellationToken);
        }

#if NET45
        public UntagQueueResponse UntagQueue(UntagQueueRequest request)
        {
            return amazonSqsToBeExtended.UntagQueue(request);
        }
#endif

        public Task<UntagQueueResponse> UntagQueueAsync(UntagQueueRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return amazonSqsToBeExtended.UntagQueueAsync(request, cancellationToken);
        }

        public IClientConfig Config => amazonSqsToBeExtended.Config;

        public ISQSPaginatorFactory Paginators => amazonSqsToBeExtended.Paginators;
    }
}
