using System.Net;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class DeleteResultContext
    {
        public DeleteResultContext(string contextId, string lineItemId, string id)
        {
            ContextId = contextId;
            Id = id;
            LineItemId = lineItemId;
            StatusCode = HttpStatusCode.OK;
        }

        public string ContextId { get; set; }
        public string Id { get; set; }
        public string LineItemId { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
