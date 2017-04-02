using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    public class ResultContainerPageResult : JsonResult
    {
        public ResultContainerPageResult(object value, int statusCode = StatusCodes.Status200OK) : base(value)
        {
            ContentType = LtiConstants.LisResultContainerMediaType;
            StatusCode = statusCode;
        }
    }
}
