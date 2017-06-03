using LtiLibrary.NetCore.Lti.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Profiles
{
    /// <summary>
    /// Represents a GetToolConsumerProfile response.
    /// </summary>
    public class GetToolConsumerProfileResponse
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public GetToolConsumerProfileResponse()
        {
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// Get or set the HTTP StatusCode.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Get or set the ToolConsumerProfile.
        /// </summary>
        public ToolConsumerProfile ToolConsumerProfile { get; set; }
    }
}
