using System.Net;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class PutResultContext
    {
        public PutResultContext(string contextId, string lineItemId, string id, LisResult result)
        {
            ContextId = contextId;
            LineItemId = lineItemId;
            Id = id;
            Result = result;
            StatusCode = HttpStatusCode.OK;
        }

        public string ContextId { get; set; }
        public string LineItemId { get; set; }
        public string Id { get; set; }
        public LisResult Result { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
