using LtiLibrary.NetCore.Lti.v1;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.Canvas.Outcomes.v1
{
    /// <summary>
    /// Represents the ReadResult response.
    /// </summary>
    public class ReadResultResponse
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public ReadResultResponse()
        {
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// The <see cref="Result"/>.
        /// </summary>
        public Result Result { get; set; }

        /// <summary>
        /// The HTTP StatusCode representing the result of the action (OK, NotFound, Unauthorized)
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Optional description of the status. This will be included in the XML response.
        /// </summary>
        public string StatusDescription { get; set; }
    }
}
