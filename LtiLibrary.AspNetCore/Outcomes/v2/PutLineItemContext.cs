using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    public class PutLineItemContext
    {
        public PutLineItemContext(LineItem lineItem)
        {
            LineItem = lineItem;
            StatusCode = StatusCodes.Status200OK;
        }

        public LineItem LineItem { get; }
        public int StatusCode { get; set; }
    }
}
