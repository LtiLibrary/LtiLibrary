using LtiLibrary.Common;
using LtiLibrary.Extensions;
using LtiLibrary.Lti1;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SimpleLti12.WebForms
{
    /// <summary>
    /// Summary description for Launch1
    /// </summary>
    public class Launch : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var ltiRequest = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
            {
                ConsumerKey = "12345",
                ResourceLinkId = "launch",
                Url = new Uri(context.Request.Url, "Tool.aspx")
                //Url = "http://localhost/LTI/tool.php"
            };

            // Tool
            ltiRequest.ToolConsumerInfoProductFamilyCode = "LtiLibrary";
            ltiRequest.ToolConsumerInfoVersion = "1.2";

            // Context
            ltiRequest.ContextId = "Home";
            ltiRequest.ContextTitle = "Home";
            ltiRequest.ContextType = LisContextType.CourseSection;

            // Instance
            ltiRequest.ToolConsumerInstanceGuid = context.Request.Url.Authority;
            ltiRequest.ToolConsumerInstanceName = "LtiLibrary Sample";
            ltiRequest.ResourceLinkTitle = "Launch";
            ltiRequest.ResourceLinkDescription = "Perform a basic LTI 1.2 launch";

            // User
            ltiRequest.LisPersonEmailPrimary = "jdoe@andyfmiller.com";
            ltiRequest.LisPersonNameFamily = "Doe";
            ltiRequest.LisPersonNameGiven = "Joan";
            ltiRequest.UserId = "1";
            ltiRequest.SetRoles(new[] { Role.Instructor });

            // Outcomes service (WebApi controller)
            var controllerUrl = UrlHelper.GenerateUrl("DefaultApi", null, "OutcomesApi", 
                new RouteValueDictionary {{ "httproute", string.Empty }}, RouteTable.Routes, 
                context.Request.RequestContext, false);
            Uri controllerUri;
            if (Uri.TryCreate(context.Request.Url, controllerUrl, out controllerUri))
            {
                ltiRequest.LisOutcomeServiceUrl = controllerUri.AbsoluteUri;
                ltiRequest.LisResultSourcedId = "ltilibrary-jdoe-1";
            }

            // Tool Consumer Profile service (WebApi controller)
            controllerUrl = UrlHelper.GenerateUrl("DefaultApi", null, "ToolConsumerProfileApi",
                new RouteValueDictionary { { "httproute", string.Empty } }, RouteTable.Routes,
                context.Request.RequestContext, false);
            if (Uri.TryCreate(context.Request.Url, controllerUrl, out controllerUri))
            {
                ltiRequest.ToolConsumerProfileUrl = controllerUri.AbsoluteUri;
                ltiRequest.AddCustomParameter("tc_profile_url", "$ToolConsumerProfile.url");
            }

            // Send the launch request to the client browser
            context.Response.WriteLtiRequest(ltiRequest, "secret");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}