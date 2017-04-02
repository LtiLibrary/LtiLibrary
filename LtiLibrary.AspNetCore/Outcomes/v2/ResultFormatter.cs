using System.Buffers;
using Microsoft.Net.Http.Headers;
using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class ResultFormatter : Microsoft.AspNetCore.Mvc.Formatters.JsonOutputFormatter
    {
        public ResultFormatter(JsonSerializerSettings settings, ArrayPool<char> charPool) : base(settings, charPool)
        {
            // Accept LineItem JSON-LD
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(LtiConstants.LisResultMediaType));
        }
    }
}
