using System.Net;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class PostLineItemContext
    {
        public PostLineItemContext(string contextId, LineItem lineItem)
        {
            ContextId = contextId;
            LineItem = lineItem;
            StatusCode = HttpStatusCode.OK;
        }

        public string ContextId { get; set; }
        public LineItem LineItem { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
