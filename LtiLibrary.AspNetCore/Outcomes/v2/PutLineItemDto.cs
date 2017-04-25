using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// Represents a PutLineItem DTO.
    /// </summary>
    public class PutLineItemDto
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public PutLineItemDto(LineItem lineItem)
        {
            LineItem = lineItem;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// Get or set the LineItem.
        /// </summary>
        public LineItem LineItem { get; }

        /// <summary>
        /// Get or set the HTTP StatusCode.
        /// </summary>
        public int StatusCode { get; set; }
    }
}
