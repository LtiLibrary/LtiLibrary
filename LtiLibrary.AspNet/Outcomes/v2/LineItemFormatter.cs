using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using LtiLibrary.Core.Common;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class LineItemFormatter : JsonMediaTypeFormatter
    {
        public LineItemFormatter()
        {
            // Accept LineItem and LineItemResults JSON-LD
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(LtiConstants.LineItemMediaType));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(LtiConstants.LineItemResultsMediaType));
        }
    }
}
