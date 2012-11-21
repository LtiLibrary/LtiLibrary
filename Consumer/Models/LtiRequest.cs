using System.Collections.Specialized;

namespace Consumer.Models
{
    public class LtiRequest
    {
        public string Action { get; set; }
        public NameValueCollection Fields { get; set; }
        public string Signature { get; set; }
        public string Title { get; set; }
    }
}