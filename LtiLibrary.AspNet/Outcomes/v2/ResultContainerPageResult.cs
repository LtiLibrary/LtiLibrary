using LtiLibrary.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class ResultContainerPageResult : JsonResult
    {
        public ResultContainerPageResult(object value) : base(value)
        {
            ContentType = LtiConstants.LisResultContainerMediaType;
        }
    }
}
