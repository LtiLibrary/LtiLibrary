using LtiLibrary.Core.Outcomes;
using System.Net.Http;

namespace LtiLibrary.Core.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        public static LtiOutcomesRequest ParseIntoLtiOutcomesRequest(this HttpRequestMessage request)
        {
            var ltiRequest = new LtiOutcomesRequest();
            ltiRequest.ParseRequest(request);
            return ltiRequest;
        }
    }
}
