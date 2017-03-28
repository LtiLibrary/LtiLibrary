using System.Net;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class DeleteLineItemContext
    {
        public DeleteLineItemContext(string contextId, string id)
        {
            ContextId = contextId;
            Id = id;
            StatusCode = StatusCodes.Status200OK;
        }

        public string ContextId { get; set; }
        public string Id { get; private set; }
        public int StatusCode { get; set; }
    }
}
