using LtiLibrary.Core.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class GetLineItemContext
    {
        public GetLineItemContext(string contextId, string id)
        {
            ContextId = contextId;
            Id = id;
            StatusCode = StatusCodes.Status200OK;
        }

        public string ContextId { get; set; }
        public string Id { get; private set; }
        public LineItem LineItem { get; set; }
        public int StatusCode { get; set; }
    }
}
