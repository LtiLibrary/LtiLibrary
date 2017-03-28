using LtiLibrary.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNet.Profiles
{
    public class ToolConsumerProfileResult : JsonResult
    {
        public ToolConsumerProfileResult(object value) : base(value)
        {
            ContentType = LtiConstants.ToolConsumerProfileMediaType;
        }
    }
}
