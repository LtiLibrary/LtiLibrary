using LtiLibrary.Common;
using Newtonsoft.Json;
using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace LtiLibrary.Profiles
{
    public class ToolConsumerProfileFormatter : JsonMediaTypeFormatter
    {
        private static readonly MediaTypeHeaderValue ToolConsumerProfileMediaType 
            = new MediaTypeHeaderValue(LtiConstants.ToolConsumerProfileMediaType);

        public ToolConsumerProfileFormatter()
        {
#if DEBUG
            // Makes it a little easier to read responses during debug and test
            Indent = true;
#endif

            // Tool Providers do not expect null values
            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            // Use the "Accept" header to invoke this formatter. If the request accepts the 
            // ToolConsumerProfileMediaType, then this formatter kicks into action.
            this.AddRequestHeaderMapping("Accept", LtiConstants.ToolConsumerProfileMediaType,
                StringComparison.OrdinalIgnoreCase, true, ToolConsumerProfileMediaType);
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            // Replace the default Content-Type header ("application/json") with the ToolConsumerProfileMediaType
            // defined by IMS. This will also set the Content-Type header in the response.
            base.SetDefaultContentHeaders(type, headers, ToolConsumerProfileMediaType);
        }
    }
}
