using System.Net;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class GetLineItemContext
    {
        public GetLineItemContext(string id)
        {
            Id = id;
            StatusCode = HttpStatusCode.OK;
        }

        public string Id { get; private set; }
        public LineItem LineItem { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
