using System.Net;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class GetResultsContext
    {
        public GetResultsContext(string contextId, string lineItemId, int? limit, int page)
        {
            ContextId = contextId;
            Limit = limit;
            LineItemId = lineItemId;
            Page = page;
            StatusCode = HttpStatusCode.OK;
        }

        public string ContextId { get; set; }
        public string LineItemId { get; set; }
        public int? Limit { get; set; }
        public ResultContainerPage ResultContainerPage { get; set; }
        public int Page { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
