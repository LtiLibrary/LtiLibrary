using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace Provider.Owin
{
    public class LtiAuthenticationMiddleware : AuthenticationMiddleware<LtiAuthenticationOptions>
    {
        private readonly ILogger _logger;

        public LtiAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, LtiAuthenticationOptions options) : base(next, options)
        {
            _logger = app.CreateLogger<LtiAuthenticationMiddleware>();
            //if (Options.StateDataFormat == null)
            //{
            //    var protector =
            //        app.CreateDataProtector(new[]
            //        {typeof (LtiAuthenticationMiddleware).FullName, Options.AuthenticationType, "v1"});
            //    Options.StateDataFormat = new PropertiesDataFormat(protector);
            //}
            //if (string.IsNullOrEmpty(Options.SignInAsAuthenticationType))
            //{
            //    Options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();
            //}
        }

        protected override AuthenticationHandler<LtiAuthenticationOptions> CreateHandler()
        {
            return new LtiAuthenticationHandler(_logger);
        }
    }
}