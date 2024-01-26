namespace Amazon.SQS.ExtendedClient
{
    using System;

    public class GuidIs3KeyProvider : IS3KeyProvider
    {
        public string GenerateName()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}