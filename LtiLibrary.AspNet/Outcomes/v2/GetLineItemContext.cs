using System.Net;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class GetLineItemContext
    {
        public GetLineItemContext(string contextId, string id)
        {
            ContextId = contextId;
            Id = id;
            StatusCode = HttpStatusCode.OK;
        }

        public string ContextId { get; set; }
        public string Id { get; private set; }
        public LineItem LineItem { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
