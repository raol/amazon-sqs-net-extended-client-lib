Amazon SQS Extended Client Library for .NET
===========================================

This is port to .NET of existing [Amazon Extended Client Library for Java](https://github.com/awslabs/amazon-sqs-java-extended-client-lib)
It enables you to store message payloads in S3 and hence overcomes message size limitation of the SQS.
With this library you can:

* Specify whether message payloads should be always stored in S3 or when message size exceeds configurable threshold.

* Send message and store its payload in S3 bucket.

* Receive stored message from S3 bucket transparently

* Delete stored payload from the S3 bucket.

## Usage

```csharp
var s3Client = new AmazonS3Client(new BasicAWSCredentials("<key>", "<secret>"), "<region>")
var sqsClient = new AmazonSQSClient("<key>", "<secret>", "<region>");
var extendedClient = new AmazonSQSExtendedClient(sqsClint, new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Client, "<s3bucketname>"));
extendedClient.SendMessage(queueUrl, "MessageBody")
```

