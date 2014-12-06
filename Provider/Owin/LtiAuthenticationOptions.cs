using Microsoft.Owin.Security;
using Provider.Owin.Provider;

namespace Provider.Owin
{
    public class LtiAuthenticationOptions : AuthenticationOptions
    {
        public LtiAuthenticationOptions() : base(LtiAuthenticationConstants.LtiAuthenticationType)
        {
            AuthenticationMode = AuthenticationMode.Passive;
        }

        /// <summary>
        /// The Provider may be assigned to an instance of an object created by the application at startup time. The middleware
        /// calls methods on the provider which give the application control at certain points where processing is occuring. 
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        //public ILtiAuthenticationProvider Provider { get; set; }

        //public string SignInAsAuthenticationType { get; set; }

        //public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
    }
}