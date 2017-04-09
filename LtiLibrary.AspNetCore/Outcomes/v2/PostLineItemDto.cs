using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// DTO to transfer a <see cref="LineItem"/> between the LineItemsControllerBase and a derived LineItemsController.
    /// </summary>
    public class PostLineItemDto
    {
        public PostLineItemDto(string contextId, LineItem lineItem)
        {
            ContextId = contextId;
            LineItem = lineItem;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// The ContextId (course) for this <see cref="LineItem"/>.
        /// </summary>
        public string ContextId { get; set; }
        /// <summary>
        /// The <see cref="LineItem"/>.
        /// </summary>
        public LineItem LineItem { get; set; }
        /// <summary>
        /// The HTTP StatusCode representing the result of the action (OK, NotFound, Unauthorized)
        /// </summary>
        public int StatusCode { get; set; }
    }
}
