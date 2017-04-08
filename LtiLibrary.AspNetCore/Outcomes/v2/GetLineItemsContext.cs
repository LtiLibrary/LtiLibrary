using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    public class GetLineItemsContext
    {
        public GetLineItemsContext(string contextId, int? limit, string activityId, int page)
        {
            ActivityId = activityId;
            ContextId = contextId;
            Limit = limit;
            Page = page;
            StatusCode = StatusCodes.Status200OK;
        }

        public string ActivityId { get; set; }
        public string ContextId { get; set; }
        public int? Limit { get; }
        public LineItemContainerPage LineItemContainerPage { get; set; }
        public int Page { get; }
        public int StatusCode { get; set; }
    }
}
