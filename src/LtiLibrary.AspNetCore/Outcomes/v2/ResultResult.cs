using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// Represents the <see cref="JsonResult"/> that is returned by the ResultController to the <see cref="ResultsControllerBase"/>.
    /// </summary>
    public class ResultResult : JsonResult
    {
        /// <summary>
        /// Initializes a new instance of the ResultResult class.
        /// </summary>
        /// <param name="value">The Result object returned by the controller.</param>
        /// <param name="statusCode">The HTTP status code returned by the controller.</param>
        public ResultResult(object value, int statusCode = StatusCodes.Status200OK) : base(value)
        {
            ContentType = LtiConstants.LisResultMediaType;
            StatusCode = statusCode;
        }
    }
}
