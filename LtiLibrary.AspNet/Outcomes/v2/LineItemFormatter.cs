using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class LineItemFormatter : JsonMediaTypeFormatter
    {
        public LineItemFormatter()
        {
            // Accept LineItem JSON-LD
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(LtiConstants.LineItemMediaType));

            // Tool Providers do not expect null values
            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}
