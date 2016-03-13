using System.Net;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class GetResultsContext
    {
        public GetResultsContext(int? limit, string lineItemId, int page)
        {
            Limit = limit;
            LineItemId = lineItemId;
            Page = page;
            StatusCode = HttpStatusCode.OK;
        }

        public string LineItemId { get; set; }
        public int? Limit { get; private set; }
        public ResultContainerPage ResultContainerPage { get; set; }
        public int Page { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
