using LtiLibrary.Core.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class GetResultContext
    {
        public GetResultContext(string contextId, string lineItemId, string id)
        {
            ContextId = contextId;
            Id = id;
            LineItemId = lineItemId;
            StatusCode = StatusCodes.Status200OK;
        }

        public string ContextId { get; set; }
        public string LineItemId { get; set; }
        public string Id { get; set; }
        public LisResult Result { get; set; }
        public int StatusCode { get; set; }
    }
}
