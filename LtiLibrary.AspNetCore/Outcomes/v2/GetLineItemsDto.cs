using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// Represents a GetLineItems DTO.
    /// </summary>
    public class GetLineItemsDto
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public GetLineItemsDto(string contextId, int? limit, string activityId, int page)
        {
            ActivityId = activityId;
            ContextId = contextId;
            Limit = limit;
            Page = page;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// Get or set the ActivityId.
        /// </summary>
        public string ActivityId { get; set; }

        /// <summary>
        /// Get or set the ContextId.
        /// </summary>
        public string ContextId { get; set; }

        /// <summary>
        /// Get or set the Limit.
        /// </summary>
        public int? Limit { get; }

        /// <summary>
        /// Get or set the LineItemContainerPage.
        /// </summary>
        public LineItemContainerPage LineItemContainerPage { get; set; }

        /// <summary>
        /// Get or set the Page.
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// Get or set the HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }
    }
}
