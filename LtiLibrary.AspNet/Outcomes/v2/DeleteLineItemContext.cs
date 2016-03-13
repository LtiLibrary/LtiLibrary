using System.Net;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class DeleteLineItemContext
    {
        public DeleteLineItemContext(string contextId, string id)
        {
            ContextId = contextId;
            Id = id;
            StatusCode = HttpStatusCode.OK;
        }

        public string ContextId { get; set; }
        public string Id { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
