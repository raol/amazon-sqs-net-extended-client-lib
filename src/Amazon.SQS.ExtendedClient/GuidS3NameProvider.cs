namespace Amazon.SQS.ExtendedClient
{
    using System;

    public class GuidS3NameProvider : IS3NamePovider
    {
        public string GenerateName()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}