using LtiLibrary.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class LineItemContainerPageResult : JsonResult
    {
        public LineItemContainerPageResult(object value) : base(value)
        {
            ContentType = LtiConstants.LineItemContainerMediaType;
        }
    }
}
