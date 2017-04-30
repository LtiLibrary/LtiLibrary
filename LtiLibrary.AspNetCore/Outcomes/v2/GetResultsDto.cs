using LtiLibrary.NetCore.Lis.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// Represents a GetResult DTO.
    /// </summary>
    public class GetResultsDto
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public GetResultsDto(string contextId, string lineItemId, int? limit, int page)
        {
            ContextId = contextId;
            Limit = limit;
            LineItemId = lineItemId;
            Page = page;
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
        /// Get or set the Limit.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Get or set the ResultContainerPage.
        /// </summary>
        public ResultContainerPage ResultContainerPage { get; set; }

        /// <summary>
        /// Get or set the Page.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Get or set the HTTP StatusCode.
        /// </summary>
        public int StatusCode { get; set; }
    }
}
