using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti.v1;

namespace LtiLibrary.NetCore.Tests.TestHelpers
{
    internal class TestLtiRequest : LtiRequest
    {
        public TestLtiRequest()
        {
        }

        public TestLtiRequest(string messageType) : base(messageType)
        {
        }

        public string CustomParameters
        {
            get
            {
                var customParameters = new UrlEncodingParser("");
                foreach (var pair in Parameters)
                {
                    if (pair.Key.StartsWith("custom_") || pair.Key.StartsWith("ext_"))
                    {
                        customParameters.Add(pair.Key, pair.Value);
                    }
                }
                return customParameters.Count == 0 ? null : customParameters.ToString();
            }
        }
    }
}
