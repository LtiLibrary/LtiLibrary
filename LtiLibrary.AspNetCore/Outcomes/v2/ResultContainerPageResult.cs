using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    public class ResultContainerPageResult : JsonResult
    {
        public ResultContainerPageResult(object value) : base(value)
        {
            ContentType = LtiConstants.LisResultContainerMediaType;
        }
    }
}
