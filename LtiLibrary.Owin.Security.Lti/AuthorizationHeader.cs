using System.Collections.Specialized;

namespace LtiLibrary.Owin.Security.Lti
{
    public class AuthorizationHeader
    {
        public AuthorizationHeader()
        {
            Parameters = new NameValueCollection();
        }

        public string Scheme { get; set; }
        public NameValueCollection Parameters { get; private set; }
    }
}
