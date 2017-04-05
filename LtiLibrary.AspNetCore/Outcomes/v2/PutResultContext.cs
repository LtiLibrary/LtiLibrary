using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    public class PutResultContext
    {
        public PutResultContext(string contextId, string lineItemId, string id, LisResult result)
        {
            ContextId = contextId;
            LineItemId = lineItemId;
            Id = id;
            Result = result;
            StatusCode = StatusCodes.Status200OK;
        }

        public string ContextId { get; set; }
        public string LineItemId { get; set; }
        public string Id { get; set; }
        public LisResult Result { get; private set; }
        public int StatusCode { get; set; }
    }
}
