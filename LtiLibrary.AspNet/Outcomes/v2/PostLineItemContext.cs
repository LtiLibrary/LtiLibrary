using LtiLibrary.Core.Outcomes;
using System.Net;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class PostLineItemContext
    {
        public PostLineItemContext(LineItem lineItem)
        {
            LineItem = lineItem;
            StatusCode = HttpStatusCode.OK;
        }

        public LineItem LineItem { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
