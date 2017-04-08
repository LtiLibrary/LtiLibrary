using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
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
        public string Id { get; }
        public LineItem LineItem { get; set; }
        public int StatusCode { get; set; }
    }
}
