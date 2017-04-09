using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Tests.SimpleHelpers;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti1;
using LtiLibrary.NetCore.OAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.Lti1
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
            Directory.SetCurrentDirectory(PlatformServices.Default.Application.ApplicationBasePath);
        }

        [Fact]
        public async void LaunchATool_WithValidCredentials()
        {
            var ltiRequest = GetLtiLaunchRequest();

            // Substitute custom variables and calculate the signature
            var signature = ltiRequest.SubstituteCustomVariablesAndGenerateSignature("secret");

            using (var response = await _client.PostAsync(ltiRequest.Url.AbsoluteUri, GetContent(ltiRequest, signature)))
            {
                Assert.True(response.IsSuccessStatusCode, $"Response status code does not indicate success: {response.StatusCode}");
                JsonAssertions.AssertSameObjectJson(await GetContentAsJObject(response), LtiConstants.BasicLaunchLtiMessageType);
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

        [Fact]
        public async void LaunchAContentItemSelectionTool_WithValidCredentials()
        {
            var ltiRequest = GetLtiContentItemSelectionRequest();

            // Substitute custom variables and calculate the signature
            var signature = ltiRequest.SubstituteCustomVariablesAndGenerateSignature("secret");

            using (var response = await _client.PostAsync(ltiRequest.Url.AbsoluteUri, GetContent(ltiRequest, signature)))
            {
                Assert.True(response.IsSuccessStatusCode, $"Response status code does not indicate success: {response.StatusCode}");
                JsonAssertions.AssertSameObjectJson(await GetContentAsJObject(response), LtiConstants.ContentItemSelectionRequestLtiMessageType);
            }
        }

        [Fact]
        public async void DoesNotLaunchAContentItemSelectionTool_WithInvalidCredentials()
        {
            var ltiRequest = GetLtiContentItemSelectionRequest();

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

        private LtiRequest GetLtiContentItemSelectionRequest()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var ltiRequest = new LtiRequest(LtiConstants.ContentItemSelectionRequestLtiMessageType)
            {
                ConsumerKey = "12345",
                ResourceLinkId = "launch",
                Url = new Uri(_client.BaseAddress, "provider/library")
            };

            ltiRequest.AcceptMediaTypes = string.Join(",", "text/html", "image/*");
            ltiRequest.AcceptPresentationDocumentTargets = string.Join(",", DocumentTarget.embed, DocumentTarget.frame);
            ltiRequest.ContentItemReturnUrl = new Uri(_client.BaseAddress, "consumer/placecontentitem").AbsoluteUri;

            return ltiRequest;
        }

        private LtiRequest GetLtiLaunchRequest()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var ltiRequest = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
            {
                ConsumerKey = "12345",
                ResourceLinkId = "launch",
                Url = new Uri(_client.BaseAddress, "provider/tool/1")
            };

            // Tool
            ltiRequest.ToolConsumerInfoProductFamilyCode = "LtiLibrary";
            ltiRequest.ToolConsumerInfoVersion = "1.2";

            // Context
            ltiRequest.ContextId = "course-1";
            ltiRequest.ContextTitle = "Course 1";
            ltiRequest.ContextType = LisContextType.CourseSection;

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
            ltiRequest.SetRoles(new[] { Role.Instructor });

            // Outcomes-1 service (WebApi controller)
            ltiRequest.LisOutcomeServiceUrl = (new Uri(_client.BaseAddress, "ims/outcomes")).AbsoluteUri;
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
            var list = request.Parameters.AllKeys.Select(key => new KeyValuePair<string, string>(key, request.Parameters[key])).ToList();
            list.Add(new KeyValuePair<string, string>(OAuthConstants.SignatureParameter, signature));
            return new FormUrlEncodedContent(list);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
