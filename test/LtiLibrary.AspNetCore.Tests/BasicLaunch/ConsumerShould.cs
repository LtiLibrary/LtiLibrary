using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Tests.SimpleHelpers;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lis.v1;
using LtiLibrary.NetCore.Lti.v1;
using LtiLibrary.NetCore.OAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.BasicLaunch
{
    public class ConsumerShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ConsumerShould()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();

            // Set the current directory to the compiler output directory so that
            // the reference json is found
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("nl-NL")]
        public async void LaunchATool_WithValidCredentials(string lcid)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(lcid);

            var ltiRequest = GetLtiLaunchRequest();

            // Substitute custom variables and calculate the signature
            var signature = ltiRequest.SubstituteCustomVariablesAndGenerateSignature("secret");

            using (var response = await _client.PostAsync(ltiRequest.Url.AbsoluteUri, GetContent(ltiRequest, signature)))
            {
                Assert.True(response.IsSuccessStatusCode, $"Response status code does not indicate success: {response.StatusCode}");
                var referenceJson = TestUtils.LoadReferenceJsonFile(LtiConstants.BasicLaunchLtiMessageType)
                    .Replace("{lcid}", lcid);
                JsonAssertions.AssertSameObjectJson(JObject.Parse(referenceJson), await GetContentAsJObject(response));
            }
        }

        [Fact]
        public async void NotLaunchATool_WithInvalidCredentials()
        {
            var ltiRequest = GetLtiLaunchRequest();

            // Substitute custom variables and calculate the signature
            var signature = ltiRequest.SubstituteCustomVariablesAndGenerateSignature("nosecret");

            using (var response = await _client.PostAsync(ltiRequest.Url.AbsoluteUri, GetContent(ltiRequest, signature)))
            {
                Assert.True(response.StatusCode == HttpStatusCode.Unauthorized, $"Response status code is not Unauthorized: {response.StatusCode}");
            }
        }

        private static async Task<JObject> GetContentAsJObject(HttpResponseMessage response)
        {
            var request = JObject.Parse(await response.Content.ReadAsStringAsync());
            // Remove temporal values that were validated by the controller when the signatures where checked
            request.Remove(OAuthConstants.NonceParameter);
            request.Remove(OAuthConstants.SignatureParameter);
            request.Remove(OAuthConstants.TimestampParameter);
            return request;
        }

        private LtiRequest GetLtiLaunchRequest()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var ltiRequest = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
            {
                ConsumerKey = "12345",
                //LaunchPresentationLocale = "en-US",
                ResourceLinkId = "launch",
                Url = new Uri(_client.BaseAddress, "toolprovider/tool/1")
            };

            // Tool
            ltiRequest.ToolConsumerInfoProductFamilyCode = "LtiLibrary";
            ltiRequest.ToolConsumerInfoVersion = "1.2";

            // Context
            ltiRequest.ContextId = "course-1";
            ltiRequest.ContextTitle = "Course 1";
            ltiRequest.ContextType = ContextType.CourseSection;

            // Instance
            ltiRequest.ToolConsumerInstanceGuid = "LtiLibrary.AspNetCore.Tests";
            ltiRequest.ToolConsumerInstanceName = "LtiLibrary Tests";
            ltiRequest.ResourceLinkTitle = "Launch";
            ltiRequest.ResourceLinkDescription = "Perform a basic LTI 1.2 launch";

            // User
            ltiRequest.LisPersonEmailPrimary = "jdoe@andyfmiller.com";
            ltiRequest.LisPersonNameFamily = "Doe";
            ltiRequest.LisPersonNameGiven = "Joan";
            ltiRequest.UserId = "1";
            ltiRequest.SetRoles(new List<Enum> { ContextRole.Instructor });

            // Outcomes-1 service (WebApi controller)
            ltiRequest.LisOutcomeServiceUrl = new Uri(_client.BaseAddress, "ims/outcomes").AbsoluteUri;
            ltiRequest.LisResultSourcedId = "testId";

            // Outcomes-2 service (WebApi controller)
            ltiRequest.LineItemServiceUrl = new Uri(_client.BaseAddress, "ims/courses/{contextId}/lineitems/{id}").AbsoluteUri;
            ltiRequest.LineItemsServiceUrl = new Uri(_client.BaseAddress, "ims/courses/{contextId}/lineitems").AbsoluteUri;
            ltiRequest.ResultServiceUrl = new Uri(_client.BaseAddress,
                "ims/courses/{contextId}/lineitems/{lineitemId}/results/{id}").AbsoluteUri;
            ltiRequest.ResultsServiceUrl = new Uri(_client.BaseAddress,
                "ims/courses/{contextId}/lineitems/{lineitemId}/results").AbsoluteUri;

            // We could just add the values here, but using parameter substitution
            // is a way to test that the correct substitions are happening
            ltiRequest.AddCustomParameter("lineitem_url", "$LineItem.url");
            ltiRequest.AddCustomParameter("lineitems_url", "$LineItems.url");
            ltiRequest.AddCustomParameter("result_url", "$Result.url");
            ltiRequest.AddCustomParameter("results_url", "$Results.url");

            // Tool Consumer Profile service (WebApi controller)
            ltiRequest.ToolConsumerProfileUrl = new Uri(_client.BaseAddress, "ims/toolconsumerprofile").AbsoluteUri;
            ltiRequest.AddCustomParameter("tc_profile_url", "$ToolConsumerProfile.url");
            return ltiRequest;
        }

        private static FormUrlEncodedContent GetContent(LtiRequest request, string signature)
        {
            var list = new List<KeyValuePair<string, string>>(request.Parameters)
            {
                new KeyValuePair<string, string>(OAuthConstants.SignatureParameter, signature)
            };
            return new FormUrlEncodedContent(list);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
