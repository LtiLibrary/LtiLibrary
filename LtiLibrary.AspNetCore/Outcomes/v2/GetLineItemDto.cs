using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// Represents a GetLineItem DTO.
    /// </summary>
    public class GetLineItemDto
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public GetLineItemDto(string contextId, string id)
        {
            ContextId = contextId;
            Id = id;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// Get or set the ContextId.
        /// </summary>
        public string ContextId { get; set; }

        /// <summary>
        /// Get or set the Id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Get or set the LineItem.
        /// </summary>
        public LineItem LineItem { get; set; }

        /// <summary>
        /// Get or set the HTTP StatusCode.
        /// </summary>
        public int StatusCode { get; set; }
    }
}
