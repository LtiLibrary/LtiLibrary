using System.Net;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class GetLineItemsContext
    {
        public GetLineItemsContext(string contextId, int? limit, string activityId, int page)
        {
            ActivityId = activityId;
            ContextId = contextId;
            Limit = limit;
            Page = page;
            StatusCode = HttpStatusCode.OK;
        }

        public string ActivityId { get; set; }
        public string ContextId { get; set; }
        public int? Limit { get; private set; }
        public LineItemContainerPage LineItemContainerPage { get; set; }
        public int Page { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
