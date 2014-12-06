using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Consumer.Lti;
using LtiLibrary.Common;
using LtiLibrary.Lti2;
using LtiLibrary.Profiles;

namespace Consumer.Controllers
{
    /// <summary>
    /// This is a sample ApiController that returns a minimal ToolConsumerProfile.
    /// </summary>
    public class ToolConsumerProfileApiController : ToolConsumerProfileApiControllerBase
    {
        protected override ToolConsumerProfile GetToolConsumerProfile(string ltiVersion)
        {
            // I use AssemblyInfo to store product and vendor values that may be used
            // in multiple places in this sample
            var guid = LtiUtility.GetProduct();
            var code = guid; // Product and ProductFamily are the same in this sample
            var vendorName = LtiUtility.GetCompany();
            var productName = LtiUtility.GetTitle();
            var productVersion = LtiUtility.GetVersion();

            // Build a minimal ToolConsumerProfile for LTI 1.2
            if (ltiVersion.Equals("LTI-1p2", StringComparison.InvariantCultureIgnoreCase))
            { 
                var capabilityOffered = new[] { LtiConstants.BasicLaunchLtiMessageType };
                var vendor = new Vendor(code, vendorName);
                var productFamily = new ProductFamily(code, vendor);
                var productInfo = new ProductInfo(productFamily, productName, productVersion);
                var productInstance = new ProductInstance(guid, productInfo);
                var profile = new ToolConsumerProfile(capabilityOffered, guid, ltiVersion, productInstance);

                // Add Outcomes Management
                var outcomesUrl = UrlHelper.GenerateUrl("DefaultApi", null, "OutcomesApi",
                    new RouteValueDictionary { { "httproute", string.Empty } }, RouteTable.Routes,
                    HttpContext.Current.Request.RequestContext, false);
                Uri outcomesUri;
                profile.ServiceOffered = new[] {
                    new RestService
                    {
                        Action = new[] { "POST" },
                        EndPoint = Uri.TryCreate(Request.RequestUri, outcomesUrl, out outcomesUri) ? outcomesUri : null,
                        Format = new[] { LtiConstants.OutcomeMediaType }
                    }
                };

                return profile;
            }

            // Otherwise, return not implemented
            return null;
        }
    }
}
