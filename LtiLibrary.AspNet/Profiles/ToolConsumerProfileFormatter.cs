using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.AspNet.Profiles
{
    public class ToolConsumerProfileFormatter : JsonMediaTypeFormatter
    {
        public ToolConsumerProfileFormatter()
        {
            // Accept ToolConsumerProfile JSON-LD
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(LtiConstants.ToolConsumerProfileMediaType));

            // Tool Providers do not expect null values
            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
#if DEBUG
            // Makes it a little easier to read responses during debug and test
            Indent = true;
#endif
        }
    }
}
