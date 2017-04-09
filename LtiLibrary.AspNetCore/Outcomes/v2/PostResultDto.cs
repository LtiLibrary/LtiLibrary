using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// DTO to transfer an <see cref="LisResult"/> between the ResultsControllerBase and a derived ResultsController.
    /// </summary>
    public class PostResultDto
    {
        public PostResultDto(string contextId, string lineItemId, LisResult result)
        {
            ContextId = contextId;
            LineItemId = lineItemId;
            Result = result;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// The ContextId (course) for this <see cref="LisResult"/>.
        /// </summary>
        public string ContextId { get; set; }
        /// <summary>
        /// The LineItemId for this <see cref="LisResult"/>.
        /// </summary>
        public string LineItemId { get; set; }
        /// <summary>
        /// The <see cref="LisResult"/> for this <see cref="LisResult"/>.
        /// </summary>
        public LisResult Result { get; set; }
        /// <summary>
        /// The HTTP StatusCode representing the result of the action (OK, NotFound, Unauthorized)
        /// </summary>
        public int StatusCode { get; set; }
    }
}
