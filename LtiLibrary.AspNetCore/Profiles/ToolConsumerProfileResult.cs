using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Profiles
{
    /// <summary>
    /// Ensure the JsonResult has the right Content-Type.
    /// </summary>
    public class ToolConsumerProfileResult : JsonResult
    {
        /// <summary>
        /// Initialize a new instance of the ToolConsumerProfileResult class.
        /// </summary>
        public ToolConsumerProfileResult(ToolConsumerProfile value) : base(value)
        {
            ContentType = LtiConstants.ToolConsumerProfileMediaType;
        }
    }
}
