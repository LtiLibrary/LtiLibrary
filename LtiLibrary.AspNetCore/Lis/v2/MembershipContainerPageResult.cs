using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Lis.v2
{
    /// <summary>
    /// Represents the MembershipContainerPage returned from the LineItemsController to the <see cref="MembershipsControllerBase"/>.
    /// </summary>
    public class MembershipContainerPageResult : JsonResult
    {
        /// <summary>
        /// Initializes a new instance of the MembershipContainerPageResult class.
        /// </summary>
        /// <param name="value">The object to return.</param>
        /// <param name="statusCode">The HTTP StatusCode to return.</param>
        public MembershipContainerPageResult(object value, int statusCode = StatusCodes.Status200OK) : base(value)
        {
            ContentType = LtiConstants.LisMembershipContainerMediaType;
            StatusCode = statusCode;
        }
    }
}
