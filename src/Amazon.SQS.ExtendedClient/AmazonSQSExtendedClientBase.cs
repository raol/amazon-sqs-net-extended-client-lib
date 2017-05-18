using System;
using Amazon.Runtime;

namespace Amazon.SQS.ExtendedClient
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;
    using Runtime.SharedInterfaces;

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

        public IClientConfig Config
        {
            get
            {
                return this.amazonSqsToBeExtended.Config;
            }
        }

        public Task<string> AuthorizeS3ToSendMessageAsync(string queueUrl, string bucket)
        {
            return amazonSqsToBeExtended.AuthorizeS3ToSendMessageAsync(queueUrl, bucket);
        }

        public Task<AddPermissionResponse> AddPermissionAsync(
            string queueUrl,
            string label,
            List<string> awsAccountIds,
            List<string> actions,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return amazonSqsToBeExtended.AddPermissionAsync(queueUrl, label, awsAccountIds, actions, cancellationToken);
        }

        public Task AddPermissionAsync(AddPermissionRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task ChangeMessageVisibilityAsync(
            string queueUrl,
            string receiptHandle,
            int visibilityTimeout,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task ChangeMessageVisibilityAsync(
            ChangeMessageVisibilityRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task ChangeMessageVisibilityBatchAsync(
            string queueUrl,
            List entries,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task ChangeMessageVisibilityBatchAsync(
            ChangeMessageVisibilityBatchRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task CreateQueueAsync(string queueName, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task CreateQueueAsync(CreateQueueRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task DeleteMessageAsync(
            string queueUrl,
            string receiptHandle,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task DeleteMessageAsync(DeleteMessageRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task DeleteMessageBatchAsync(
            string queueUrl,
            List entries,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task DeleteMessageBatchAsync(
            DeleteMessageBatchRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task DeleteQueueAsync(string queueUrl, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task DeleteQueueAsync(DeleteQueueRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task GetQueueAttributesAsync(
            string queueUrl,
            List attributeNames,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task GetQueueAttributesAsync(
            GetQueueAttributesRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task GetQueueUrlAsync(string queueName, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task GetQueueUrlAsync(GetQueueUrlRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task ListDeadLetterSourceQueuesAsync(
            ListDeadLetterSourceQueuesRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task ListQueuesAsync(string queueNamePrefix, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task ListQueuesAsync(ListQueuesRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task PurgeQueueAsync(string queueUrl, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task PurgeQueueAsync(PurgeQueueRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task ReceiveMessageAsync(string queueUrl, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task ReceiveMessageAsync(ReceiveMessageRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task RemovePermissionAsync(string queueUrl, string label, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task RemovePermissionAsync(
            RemovePermissionRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task SendMessageAsync(
            string queueUrl,
            string messageBody,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task SendMessageBatchAsync(string queueUrl, List entries, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task SendMessageBatchAsync(
            SendMessageBatchRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task SetQueueAttributesAsync(
            string queueUrl,
            Dictionary attributes,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task SetQueueAttributesAsync(
            SetQueueAttributesRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}
