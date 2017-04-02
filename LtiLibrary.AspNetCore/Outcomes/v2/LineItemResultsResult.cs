using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    public class LineItemResultsResult : JsonResult
    {
        public LineItemResultsResult(object value) : base(value)
        {
            ContentType = LtiConstants.LineItemResultsMediaType;
        }
    }
}
