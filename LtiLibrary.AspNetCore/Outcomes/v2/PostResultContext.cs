using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    public class PostResultContext
    {
        public PostResultContext(string contextId, string lineItemId, LisResult result)
        {
            ContextId = contextId;
            LineItemId = lineItemId;
            Result = result;
            StatusCode = StatusCodes.Status200OK;
        }

        public string ContextId { get; set; }
        public string LineItemId { get; set; }
        public LisResult Result { get; set; }
        public int StatusCode { get; set; }
    }
}
