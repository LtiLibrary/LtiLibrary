using LtiLibrary.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class ResultResult : JsonResult
    {
        public ResultResult(object value) : base(value)
        {
            ContentType = LtiConstants.LisResultMediaType;
        }
    }
}
