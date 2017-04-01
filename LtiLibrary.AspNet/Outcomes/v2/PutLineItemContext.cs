using LtiLibrary.Core.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class PutLineItemContext
    {
        public PutLineItemContext(LineItem lineItem)
        {
            LineItem = lineItem;
            StatusCode = StatusCodes.Status200OK;
        }

        public LineItem LineItem { get; private set; }
        public int StatusCode { get; set; }
    }
}
