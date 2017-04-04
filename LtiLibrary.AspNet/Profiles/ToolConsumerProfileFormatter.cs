using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using LtiLibrary.Core.Common;

namespace LtiLibrary.AspNet.Profiles
{
    public class ToolConsumerProfileFormatter : JsonMediaTypeFormatter
    {
        public ToolConsumerProfileFormatter()
        {
            // Accept ToolConsumerProfile JSON-LD
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(LtiConstants.ToolConsumerProfileMediaType));
        }
    }
}
