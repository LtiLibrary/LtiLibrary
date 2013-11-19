using LtiLibrary.Common;
using LtiLibrary.Consumer;

namespace Consumer.Controllers
{
    public class ToolConsumerProfileController : ToolConsumerProfileControllerBase
    {
// ReSharper disable InconsistentNaming
        public override ToolConsumerProfile Get(string lti_version = "LTI-1p0")
// ReSharper restore InconsistentNaming
        {
            var guid = Request.RequestUri.ToString();
            var context = new[] { LtiConstants.ToolConsumerProfileContext };
            var vendor = new Vendor("samples", "Andy F. Miller");
            var productFamily = new ProductFamily("samples", vendor);
            var productInfo = new ProductInfo(productFamily, "sample consumer", "1.2");
            var productInstance = new ProductInstance(guid, productInfo);
            var capabilityOffered = new[] { LtiConstants.LtiMessageType };
            var profile = new ToolConsumerProfile(context, capabilityOffered, guid,
                                                  lti_version, productInstance, null);
            return profile;
        }
    }
}
