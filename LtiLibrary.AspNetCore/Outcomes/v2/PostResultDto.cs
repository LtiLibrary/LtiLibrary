using LtiLibrary.NetCore.Lis.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// DTO to transfer an <see cref="NetCore.Lis.v2.Result"/> between the ResultsControllerBase and a derived ResultsController.
    /// </summary>
    public class PostResultDto
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public PostResultDto(string contextId, string lineItemId, Result result)
        {
            ContextId = contextId;
            LineItemId = lineItemId;
            Result = result;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// The ContextId (course) for this <see cref="NetCore.Lis.v2.Result"/>.
        /// </summary>
        public string ContextId { get; set; }
        /// <summary>
        /// The LineItemId for this <see cref="NetCore.Lis.v2.Result"/>.
        /// </summary>
        public string LineItemId { get; set; }
        /// <summary>
        /// The <see cref="NetCore.Lis.v2.Result"/> for this <see cref="NetCore.Lis.v2.Result"/>.
        /// </summary>
        public Result Result { get; set; }
        /// <summary>
        /// The HTTP StatusCode representing the result of the action (OK, NotFound, Unauthorized)
        /// </summary>
        public int StatusCode { get; set; }
    }
}
