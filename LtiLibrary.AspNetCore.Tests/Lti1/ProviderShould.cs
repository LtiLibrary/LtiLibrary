using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti1;
using LtiLibrary.NetCore.OAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.Lti1
{
    public class ProviderShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ProviderShould()
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

            // Substitute custom variables and calculate the signature
            var signature = ltiRequest.SubstituteCustomVariablesAndGenerateSignature("secret");

            using (var response = await _client.PostAsync("provider/tool/1", GetContent(ltiRequest, signature)))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        private FormUrlEncodedContent GetContent(LtiRequest request, string signature)
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
