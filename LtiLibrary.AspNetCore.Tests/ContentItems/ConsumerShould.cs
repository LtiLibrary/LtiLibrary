using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Tests.SimpleHelpers;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lti.v1;
using LtiLibrary.NetCore.OAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.ContentItems
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
        public async void LaunchAContentItemSelectionTool_WithValidCredentials()
        {
            var ltiRequest = GetLtiContentItemSelectionRequest("contentitemsprovider/library");

            // Substitute custom variables and calculate the signature
            var signature = ltiRequest.SubstituteCustomVariablesAndGenerateSignature("secret");

            using (var response = await _client.PostAsync(ltiRequest.Url.AbsoluteUri, GetContent(ltiRequest, signature)))
            {
                Assert.True(response.IsSuccessStatusCode, $"Response status code does not indicate success: {response.StatusCode}");
                JsonAssertions.AssertSameObjectJson(await GetContentAsJObject(response), LtiConstants.ContentItemSelectionRequestLtiMessageType);
            }
        }

        [Fact]
        public async void NotLaunchAContentItemSelectionTool_WithInvalidCredentials()
        {
            var ltiRequest = GetLtiContentItemSelectionRequest("contentitemsprovider/library");

            // Substitute custom variables and calculate the signature
            var signature = ltiRequest.SubstituteCustomVariablesAndGenerateSignature("nosecret");

            using (var response = await _client.PostAsync(ltiRequest.Url.AbsoluteUri, GetContent(ltiRequest, signature)))
            {
                Assert.True(response.StatusCode == HttpStatusCode.Unauthorized, $"Response status code is not Unauthorized: {response.StatusCode}");
            }
        }

        [Fact]
        public async void NotLaunchAContentItemSelectionTool_WithInvalidMessageType()
        {
            var ltiRequest = GetLtiContentItemSelectionRequest("contentitemsprovider/library");
            ltiRequest.LtiMessageType = LtiConstants.BasicLaunchLtiMessageType;

            // Substitute custom variables and calculate the signature
            var signature = ltiRequest.SubstituteCustomVariablesAndGenerateSignature("secret");

            using (var response = await _client.PostAsync(ltiRequest.Url.AbsoluteUri, GetContent(ltiRequest, signature)))
            {
                Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async void GetContentItemSelection()
        {
            var ltiRequest = GetLtiContentItemSelectionRequest("contentitemsprovider/librarythatreturnsselection");

            // Substitute custom variables and calculate the signature
            var signature = ltiRequest.SubstituteCustomVariablesAndGenerateSignature("secret");

            using (var response = await _client.PostAsync(ltiRequest.Url.AbsoluteUri, GetContent(ltiRequest, signature)))
            {
                Assert.True(response.IsSuccessStatusCode, $"Response status code does not indicate success: {response.StatusCode}\n{response.Content.ReadAsStringAsync().Result}");

                var selection = await GetContentAsJObject(response);
                Assert.Equal(LtiConstants.ContentItemSelectionLtiMessageType, selection[LtiConstants.LtiMessageTypeParameter]);
                Assert.Equal("VeryImportantData", selection[LtiConstants.ContentItemDataParameter]);

                var graph = JObject.Parse(selection[LtiConstants.ContentItemPlacementParameter].Value<string>());
                JsonAssertions.AssertSameObjectJson(graph, LtiConstants.ContentItemSelectionLtiMessageType);
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

        /// <summary>
        /// Create a selection with 2 LtiLinks
        /// </summary>
        private LtiRequest GetLtiContentItemSelectionRequest(string url)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var ltiRequest = new LtiRequest(LtiConstants.ContentItemSelectionRequestLtiMessageType)
            {
                ConsumerKey = "12345",
                ResourceLinkId = "launch",
                Url = new Uri(_client.BaseAddress, url)
            };

            ltiRequest.AcceptMediaTypes = string.Join(",", "text/html", "image/*");
            ltiRequest.AcceptPresentationDocumentTargets = string.Join(",", DocumentTarget.embed, DocumentTarget.frame);
            ltiRequest.ContentItemReturnUrl = new Uri(_client.BaseAddress, "consumer/placecontentitem").AbsoluteUri;
            ltiRequest.Data = "VeryImportantData";

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
