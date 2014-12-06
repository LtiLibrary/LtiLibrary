using System;

namespace Provider.Lti
{
    public class ProviderRequest
    {
        public int ProviderRequestId { get; set; }

        public string LtiRequest { get; set; }
        public DateTime Received { get; set; }
    }
}
