using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    public class LineItemResult : JsonResult
    {
        public LineItemResult(object value) : base(value)
        {
            ContentType = LtiConstants.LineItemMediaType;
        }
    }
}
