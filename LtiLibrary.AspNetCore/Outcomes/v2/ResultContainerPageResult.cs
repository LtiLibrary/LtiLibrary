using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// Represents an IMS ResultContainerPage object.
    /// </summary>
    public class ResultContainerPageResult : JsonResult
    {
        /// <summary>
        /// Initialize a new instance of the ResultContainerPageResult class.
        /// </summary>
        public ResultContainerPageResult(object value, int statusCode = StatusCodes.Status200OK) : base(value)
        {
            ContentType = LtiConstants.LisResultContainerMediaType;
            StatusCode = statusCode;
        }
    }
}
