using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using LtiLibrary.Core.Common;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class LineItemContainerPageFormatter : JsonMediaTypeFormatter
    {
        public LineItemContainerPageFormatter()
        {
            // Accept LineItemContainerPage JSON-LD
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(LtiConstants.LineItemContainerMediaType));
        }
    }
}
