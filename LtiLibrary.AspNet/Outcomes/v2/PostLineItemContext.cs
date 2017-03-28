using System.Net;
using LtiLibrary.Core.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class PostLineItemContext
    {
        public PostLineItemContext(string contextId, LineItem lineItem)
        {
            ContextId = contextId;
            LineItem = lineItem;
            StatusCode = StatusCodes.Status200OK;
        }

        public string ContextId { get; set; }
        public LineItem LineItem { get; set; }
        public int StatusCode { get; set; }
    }
}
