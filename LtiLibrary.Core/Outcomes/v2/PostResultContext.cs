using System.Net;

namespace LtiLibrary.Core.Outcomes.v2
{
    public class PostResultContext
    {
        public PostResultContext(string contextId, string lineItemId, LisResult result)
        {
            ContextId = contextId;
            LineItemId = lineItemId;
            Result = result;
            StatusCode = HttpStatusCode.OK;
        }

        public string ContextId { get; set; }
        public string LineItemId { get; set; }
        public LisResult Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
