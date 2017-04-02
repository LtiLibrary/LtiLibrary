using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using LtiLibrary.Core.Common;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class ResultContainerPageFormatter : JsonMediaTypeFormatter
    {
        public ResultContainerPageFormatter()
        {
            // Accept ResultContainerPage JSON-LD
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(LtiConstants.LisResultContainerMediaType));
        }
    }
}
