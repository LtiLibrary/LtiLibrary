using LtiLibrary.Common;
using LtiLibrary.Lti2;
using LtiLibrary.Profiles;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SimpleLti12.Controllers
{
    /// <summary>
    /// The ToolConsumerProfileApiController is hosted by the Tool Consumer and
    /// provides the Tool Consumer Profile API specified by IMS LTI 1.2 (and 2.0).
    /// </summary>
    public class ToolConsumerProfileApiController : ToolConsumerProfileApiControllerBase
    {
        /// <summary>
        /// Gets a Tool Consumer Profile to return to the Tool Provider.
        /// </summary>
        /// <param name="ltiVersion">The version of LTI supported by this Tool Consumer.</param>
        /// <returns>A ToolConsumerProfile that describes this Tool Consumer.</returns>
        protected override ToolConsumerProfile GetToolConsumerProfile(string ltiVersion)
        {
            // I use AssemblyInfo to store product and vendor values that may be used
            // in multiple places in this sample
            const string guid = "LtiLibrarySample";
            const string code = "LtiLibrary";
            const string vendorName = "andyfmiller.com";
            const string productName = "LtiLibrary Sample";
            const string productVersion = "1.2";

            // Build a minimal ToolConsumerProfile for LTI 1.2
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
    }
}
