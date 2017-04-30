using LtiLibrary.NetCore.Lti.v1;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// Represents a ReplaceResult DTO.
    /// </summary>
    public class ReplaceResultDto
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public ReplaceResultDto(Result result)
        {
            Result = result;
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

    }
}
