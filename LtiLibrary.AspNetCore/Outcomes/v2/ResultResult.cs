using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    public class ResultResult : JsonResult
    {
        public ResultResult(object value) : base(value)
        {
            ContentType = LtiConstants.LisResultMediaType;
        }
    }
}
