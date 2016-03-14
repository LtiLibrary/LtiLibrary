using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using LtiLibrary.Core.Common;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class LineItemsContainerPageFormatter : JsonMediaTypeFormatter
    {
        public LineItemsContainerPageFormatter()
        {
            // Accept LineItem JSON-LD
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(LtiConstants.LineItemContainerMediaType));
        }
    }
}
