using LtiLibrary.OAuth;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Provider.Lti;
using Provider.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Provider.Owin
{
    public class LtiAuthenticationHandler : AuthenticationHandler<LtiAuthenticationOptions>
    {
        private readonly ILogger _logger;

        public LtiAuthenticationHandler(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _logger = logger;
        }

        public ProviderContext ProviderContext
        {
            get
            {
                return Context.Get<ProviderContext>();
            }
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationTicket ticket = null;

            try
            {
                if (!string.Equals(Request.Method, "POST", StringComparison.OrdinalIgnoreCase) 
                    || string.IsNullOrWhiteSpace(Request.ContentType) 
                    || !Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) 
                    || !Request.Body.CanRead) return null;


                if (!Request.Body.CanSeek)
                {
                    // Buffer in case this body was not meant for us.
                    MemoryStream memoryStream = new MemoryStream();
                    await Request.Body.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    Request.Body = memoryStream;
                }
                var form = await Request.ReadFormAsync();
                Request.Body.Seek(0, SeekOrigin.Begin);

                if (!form.IsAuthenticatedWithLti()) return null;

                // Capture the request
                ProviderRequest providerRequest;
                try
                {
                    form.CheckForRequiredLtiParameters();
                    providerRequest = new ProviderRequest();
                    providerRequest.HttpMethod = Request.Method;
                    providerRequest.Url = Request.Uri;
                    providerRequest.Parameters.Add(form.AsNameValueCollection());

                    // Find the consumer (throws an LtiException if consumer not found)
                    var consumer = ProviderContext.Consumers.SingleOrDefault(c => c.Key == providerRequest.ConsumerKey);
                    if (consumer == null)
                    {
                        _logger.WriteInformation("Invalid " + OAuthConstants.ConsumerKeyParameter);
                        return null;
                    }

                    // Make sure the signature is valid
                    var oauthSignature = Request.GenerateOAuthSignature(form, consumer.Secret);
                    if (!oauthSignature.Equals(providerRequest.Signature))
                    {
                        var signatureBase = Request.GenerateOAuthSignatureBase(form);
                        _logger.WriteInformation("Signature base string: " + signatureBase);
                        _logger.WriteError("Invalid " + OAuthConstants.SignatureParameter);
                        return null;
                    }

                    // Make sure the request is not being replayed
                    var timeout = TimeSpan.FromMinutes(5);
                    var oauthTimestampAbsolute = OAuthConstants.Epoch.AddSeconds(providerRequest.Timestamp);
                    if (DateTime.UtcNow - oauthTimestampAbsolute > timeout)
                    {
                        _logger.WriteError("Expired " + OAuthConstants.TimestampParameter);
                        return null;
                    }
                    if (null != ProviderContext.ProviderRequests.SingleOrDefault(r => r.Nonce == providerRequest.Nonce))
                    {
                        _logger.WriteError("Nonce already used");
                    }

                    // No point in keeping very old requests
                    var oldestTimestamp =
                        Convert.ToInt64(((DateTime.UtcNow - timeout) - OAuthConstants.Epoch).TotalSeconds);
                    foreach (
                        var oldRequest in
                            ProviderContext.ProviderRequests.Where(n => n.Timestamp < oldestTimestamp).ToList())
                    {
                        ProviderContext.ProviderRequests.Remove(oldRequest);
                    }

                    // Save the request
                    providerRequest.ConsumerId = consumer.ConsumerId;
                    ProviderContext.ProviderRequests.Add(providerRequest);
                    ProviderContext.SaveChanges();

                    // Outcomes can live a long time to give the teacher enough
                    // time to grade the assignment. So they are stored in a separate table.
                    var lisOutcomeServiceUrl = providerRequest.LisOutcomeServiceUrl;
                    var lisResultSourcedid = providerRequest.LisResultSourcedId;
                    if (!string.IsNullOrWhiteSpace(lisOutcomeServiceUrl) &&
                        !string.IsNullOrWhiteSpace(lisResultSourcedid))
                    {
                        var outcome = ProviderContext.Outcomes.SingleOrDefault(o =>
                            o.ConsumerId == providerRequest.ConsumerId
                            && o.LisResultSourcedId == lisResultSourcedid);

                        if (outcome == null)
                        {
                            outcome = new Outcome
                            {
                                ConsumerId = providerRequest.ConsumerId,
                                LisResultSourcedId = lisResultSourcedid
                            };
                            ProviderContext.Outcomes.Add(outcome);
                            ProviderContext.SaveChanges(); // Assign OutcomeId;
                        }
                        outcome.ContextTitle = providerRequest.ContextTitle;
                        outcome.ServiceUrl = lisOutcomeServiceUrl;
                        providerRequest.OutcomeId = outcome.OutcomeId;
                        ProviderContext.SaveChanges();
                    }
                }
                catch (Exception error)
                {
                    var exceptionContext = new LtiAuthenticationExceptionContext(Context, Options,
                        LtiAuthenticationExceptionContext.ExceptionLocation.AuthenticateAsync,
                        error, null);
                    if (exceptionContext.Rethrow)
                    {
                        throw;
                    }
                    return exceptionContext.Ticket;
                }

                var identity = new ClaimsIdentity(Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier,
                    providerRequest.UserId,
                    null,
                    Options.AuthenticationType));
                if (!string.IsNullOrEmpty(providerRequest.UserName))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Name,
                        providerRequest.UserName));
                }
                else
                {
                    identity.AddClaim(new Claim(ClaimTypes.Name,
                        providerRequest.UserId));
                }
                if (!string.IsNullOrEmpty(providerRequest.LisPersonEmailPrimary))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Email,
                        providerRequest.LisPersonEmailPrimary));
                }
                if (!string.IsNullOrEmpty(providerRequest.LisPersonNameGiven))
                {
                    identity.AddClaim(new Claim(ClaimTypes.GivenName,
                        providerRequest.LisPersonNameGiven));
                }
                if (!string.IsNullOrEmpty(providerRequest.LisPersonNameFamily))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Surname,
                        providerRequest.LisPersonNameFamily));
                }
                identity.AddClaim(new Claim(LtiAuthenticationConstants.LtiRequestIdUrn,
                    providerRequest.ProviderRequestId.ToString(CultureInfo.InvariantCulture)));
                var properties = new AuthenticationProperties();

                ticket = new AuthenticationTicket(identity, properties);

                return ticket;
            }
            catch (Exception exception)
            {
                var exceptionContext = new LtiAuthenticationExceptionContext(Context, Options,
                    LtiAuthenticationExceptionContext.ExceptionLocation.AuthenticateAsync,
                    exception, ticket);
                if (exceptionContext.Rethrow)
                {
                    throw;
                }
                return exceptionContext.Ticket;
            }
        }

        //public async override Task<bool> InvokeAsync()
        //{
        //    if (!string.Equals(Request.Method, "POST", StringComparison.OrdinalIgnoreCase)
        //        || string.IsNullOrWhiteSpace(Request.ContentType)
        //        || !Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase)
        //        || !Request.Body.CanRead) return false;

        //    if (!Request.Body.CanSeek)
        //    {
        //        // Buffer in case this body was not meant for us.
        //        MemoryStream memoryStream = new MemoryStream();
        //        await Request.Body.CopyToAsync(memoryStream);
        //        memoryStream.Seek(0, SeekOrigin.Begin);
        //        Request.Body = memoryStream;
        //    }
        //    var form = await Request.ReadFormAsync();
        //    Request.Body.Seek(0, SeekOrigin.Begin);

        //    if (!form.IsAuthenticatedWithLti()) return false;

        //    AuthenticationTicket ticket = await AuthenticateAsync();
        //    if (ticket == null)
        //    {
        //        return false;
        //    }

        //    Context.Authentication.SignIn(ticket.Properties, new[] {ticket.Identity});
        //    return true;
        //}
    }
}