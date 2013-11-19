using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using LtiLibrary.Common;

namespace LtiLibrary.Consumer
{
    public class ToolConsumerProfileFormatter : JsonMediaTypeFormatter
    {
        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            // Replace the default media type ("application/json") with the specific ToolConsumerProfile media type
            // defined by IMS. This will also set the Content-Type header in the response.
            var profileMediaType = new MediaTypeHeaderValue(LtiConstants.ToolConsumerProfileMediaType);

            base.SetDefaultContentHeaders(type, headers, profileMediaType);
        }
    }
}
