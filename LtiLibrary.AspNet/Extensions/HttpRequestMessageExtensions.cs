using System.Net.Http;
using LtiLibrary.AspNet.Outcomes;

namespace LtiLibrary.AspNet.Extensions
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
