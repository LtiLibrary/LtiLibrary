using LtiLibrary.AspNetCore.Tests.SimpleHelpers;
using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Tests.Membership
{
    [Produces(LtiConstants.LisMembershipContainerMediaType)]
    [Route("ims/[controller]/{referenceFileName}", Name = "RawMembershipApi")]
    public class RawMembershipController : Controller
    {
        public IActionResult GetMemberships(string referenceFileName)
        {
            var result = new ContentResult
            {
                Content = TestUtils.LoadReferenceJsonFile(referenceFileName),
                ContentType = LtiConstants.LisMembershipContainerMediaType
            };
            return result;
        }
    }
}
