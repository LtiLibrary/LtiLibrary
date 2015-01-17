using LtiLibrary.Core.Outcomes;
using System.Net;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class PutLineItemContext
    {
        public PutLineItemContext(LineItem lineItem)
        {
            LineItem = lineItem;
            StatusCode = HttpStatusCode.OK;
        }

        public LineItem LineItem { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
