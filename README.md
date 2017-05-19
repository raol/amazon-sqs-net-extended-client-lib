Amazon SQS Extended Client Library for .NET Core
===========================================

[![Build status](https://ci.appveyor.com/api/projects/status/ljru25gu01h0rd8l/branch/master?svg=true)](https://ci.appveyor.com/project/raol/amazon-sqs-net-extended-client-lib)

This project was forked from [Amazon SQS Extended Client Library for .NET](https://github.com/raol/amazon-sqs-net-extended-client-lib),
which is a port to .NET of existing [Amazon Extended Client Library for Java](https://github.com/awslabs/amazon-sqs-java-extended-client-lib)
It enables you to store message payloads in S3 and hence overcomes message size limitation of the SQS.
With this library you can:

* Specify whether message payloads should be always stored in S3 or when message size exceeds configurable threshold.

* Send message and store its payload in S3 bucket.

* Receive stored message from S3 bucket transparently

* Delete stored payload from the S3 bucket.

## Installation

[![Nuget](https://img.shields.io/nuget/v/Amazon.SQS.ExtendedClient.svg?style=flat)](https://www.nuget.org/packages/Amazon.SQS.ExtendedClient/)

To install via nuget, run following command in the Package Manager Console
```PowerShell
Install-Package Amazon.SQS.ExtendedClient.Core
```

## Usage

```csharp
var s3Client = new AmazonS3Client(new BasicAWSCredentials("<key>", "<secret>"), "<region>")
var sqsClient = new AmazonSQSClient(new BasicAWSCredentials("<key>", "<secret>"), "<region>");
var extendedClient = new AmazonSQSExtendedClient(
    sqsClient, 
    new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Client, "<s3bucketname>"));
extendedClient.SendMessage(queueUrl, "MessageBody")
```

### Notes

* message size threshold is an integer value in bytes

* the current default AWS SQS message size limitation is set to 262144 bytes