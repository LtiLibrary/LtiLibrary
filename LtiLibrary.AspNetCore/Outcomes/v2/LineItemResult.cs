using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// Represents the <see cref="JsonResult"/> returned by the LineItemsController to the <see cref="LineItemsControllerBase"/>
    /// </summary>
    public class LineItemResult : JsonResult
    {
        /// <summary>
        /// Initialize a new instance of the LineItemResult.
        /// </summary>
        /// <param name="value">The object returned by the controller."/></param>
        /// <param name="statusCode">The HTTP status code returned by the controller.</param>
        public LineItemResult(object value, int statusCode = StatusCodes.Status200OK) : base(value)
        {
            ContentType = LtiConstants.LineItemMediaType;
            StatusCode = statusCode;
        }
    }
}
