using LtiLibrary.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class LineItemResultsResult : JsonResult
    {
        public LineItemResultsResult(object value) : base(value)
        {
            ContentType = LtiConstants.LineItemResultsMediaType;
        }
    }
}
