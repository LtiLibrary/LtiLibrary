using System.Collections.Specialized;

namespace LtiLibrary.AspNetCore.Lti1
{
    public class LtiRequestViewModel
    {
        public string Action { get; set; }
        public NameValueCollection Fields { get; set; }
        public string Signature { get; set; }
    }
}
