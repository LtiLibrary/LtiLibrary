using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    public class LineItemContainerPageResult : JsonResult
    {
        public LineItemContainerPageResult(object value) : base(value)
        {
            ContentType = LtiConstants.LineItemContainerMediaType;
        }
    }
}
