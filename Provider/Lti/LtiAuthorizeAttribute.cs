using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LtiLibrary.Models;
using LtiLibrary.Provider;
using Provider.Models;

namespace Provider.Lti
{
    /// <summary>
    /// Add this to a controller or an action within a controller that should receives
    /// LTI requests from a consumer. If the request is not authenticated or not
    /// authorized, the action/s will not be executed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
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

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = HttpContext.Current.Request;

            if (!LtiInboundRequestHandler.IsAuthenticatedWithLti(request)) return;

            LtiInboundRequest ltiRequest;
            using (var handler = new LtiInboundRequestHandler())
            {
                try
                {
                    ltiRequest = handler.Parse(request);
                }
                catch (Exception ex)
                {
                    filterContext.Result = new RedirectResult(BadRequestUrl + "?error=" + ex.Message);
                    return;
                }
            }
            filterContext.Controller.TempData["LtiInboundRequestId"] = ltiRequest.LtiInboundRequestId;

            using (var db = new ProviderContext())
            {
                var pairedUser = db.PairedUsers.SingleOrDefault(
                    u => u.ConsumerId == ltiRequest.ConsumerId && u.ConsumerUserId == ltiRequest.UserId);

                if (pairedUser == null)
                {
                    var url = UnauthenticatedUrl +
                              "?ReturnUrl=" + HttpUtility.UrlEncode(ltiRequest.Url) +
                              "&RequestId=" + ltiRequest.LtiInboundRequestId.ToString(CultureInfo.InvariantCulture);
                    filterContext.Result = new RedirectResult(url);
                }
                else
                {
                    LtiSecurity.Login(pairedUser);
                }
            }
        }
    }
}
