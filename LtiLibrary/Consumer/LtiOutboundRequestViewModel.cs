using System.Collections.Specialized;

namespace LtiLibrary.Consumer
{
    public class LtiOutboundRequestViewModel
    {
        public string Action { get; set; }
        public NameValueCollection Fields { get; set; }
        public string Signature { get; set; }
        public string Title { get; set; }
    }
}
