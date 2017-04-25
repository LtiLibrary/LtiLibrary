using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// Represents a PutResult DTO.
    /// </summary>
    public class PutResultDto
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public PutResultDto(string contextId, string lineItemId, string id, LisResult result)
        {
            ContextId = contextId;
            LineItemId = lineItemId;
            Id = id;
            Result = result;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// Get or set the ContextId.
        /// </summary>
        public string ContextId { get; set; }

        /// <summary>
        /// Get or set the LineItemId.
        /// </summary>
        public string LineItemId { get; set; }

        /// <summary>
        /// Get or set the Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Get or set the Result.
        /// </summary>
        public LisResult Result { get; }

        /// <summary>
        /// Get or set the HTTP StatusCode.
        /// </summary>
        public int StatusCode { get; set; }
    }
}
