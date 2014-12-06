using Consumer.Models;
using LtiLibrary.Common;
using LtiLibrary.Extensions;
using LtiLibrary.Lti1;
using LtiLibrary.OAuth;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Consumer.Lti
{
    /// <summary>
    /// Add this to a controller or an action within a controller that receives
    /// LTI launch requests from a consumer. If the request is not authenticated
    /// or not authorized, the action/s will not be executed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class LtiAuthorizeAttribute : AuthorizeAttribute
    {
        private string _badrequestUrl = "/Error/BadRequest";
        private string _unauthorizedUrl = "/Tool/Unauthorized/{id}";
        private string _unauthenticatedUrl = "/Account/LtiLoginConfirmation";

        public string BadRequestUrl
        {
            get { return _badrequestUrl; }
            set { _badrequestUrl = value; }
        }

        public string UnauthenticatedUrl
        {
            get { return _unauthenticatedUrl; }
            set { _unauthenticatedUrl = value; }
        }

        public string UnauthorizedUrl
        {
            get { return _unauthorizedUrl; }
            set { _unauthorizedUrl = value; }
        }

        public override void OnAuthorization(AuthorizationContext authorizationContext)
        {
            var request = authorizationContext.HttpContext.Request;

            if (!request.IsAuthenticatedWithLti()) return;

            using (var db = ConsumerContext.Create())
            {
                try
                {
                    request.CheckForRequiredLtiParameters();
                    var ltiRequest = new LtiRequest();
                    ltiRequest.ParseRequest(request, !authorizationContext.Controller.ValidateRequest);

                    // Find the consumer (throws an LtiException if consumer not found)
                    var contentItemTool = db.ContentItemTools.SingleOrDefault(c => c.ConsumerKey == ltiRequest.ConsumerKey);
                    if (contentItemTool == null)
                    {
                        throw new LtiException("Invalid " + OAuthConstants.ConsumerKeyParameter);
                    }

                    // Make sure the signature is valid
                    var oauthSignature = request.GenerateOAuthSignature(contentItemTool.ConsumerSecret);
                    if (!oauthSignature.Equals(ltiRequest.Signature))
                    {
                        var signatureBase = request.GenerateOAuthSignatureBase();
                        throw new LtiException("Invalid " + OAuthConstants.SignatureParameter,
                            new LtiException("Signature base string: " + signatureBase));
                    }
                }
                catch (Exception ex)
                {
                    authorizationContext.Result = new RedirectResult(BadRequestUrl + "?error=" + ex.Message);
                }
            }
        }
    }
}
