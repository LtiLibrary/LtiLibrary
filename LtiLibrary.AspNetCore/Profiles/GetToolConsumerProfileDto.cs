using LtiLibrary.NetCore.Profiles;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Profiles
{
    /// <summary>
    /// Represents a GetToolConsumerProfile DTO.
    /// </summary>
    public class GetToolConsumerProfileDto
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public GetToolConsumerProfileDto(string ltiVersion)
        {
            LtiVersion = ltiVersion;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// Get or set the LtiVersion.
        /// </summary>
        public string LtiVersion { get; }

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
