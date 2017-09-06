using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// Represents the LineItemContainerPage returned from the LineItemsController to the <see cref="LineItemsControllerBase"/>.
    /// </summary>
    public class LineItemContainerPageResult : JsonResult
    {
        /// <summary>
        /// Initializes a new instance of the LineItemContainerPageResult class.
        /// </summary>
        /// <param name="value">The object to return.</param>
        /// <param name="statusCode">The HTTP StatusCode to return.</param>
        public LineItemContainerPageResult(object value, int statusCode = StatusCodes.Status200OK) : base(value)
        {
            ContentType = LtiConstants.LisLineItemContainerMediaType;
            StatusCode = statusCode;
        }
    }
}
