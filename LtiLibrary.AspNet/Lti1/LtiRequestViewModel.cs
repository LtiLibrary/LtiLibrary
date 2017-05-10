using System.Collections.Specialized;

namespace LtiLibrary.AspNet.Lti1
{
    public class LtiRequestViewModel
    {
        public string Action { get; set; }
        public NameValueCollection Fields { get; set; }
    }
}
