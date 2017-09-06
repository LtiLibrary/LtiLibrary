using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using LtiLibrary.AspNetCore.Tests.SimpleHelpers;
using LtiLibrary.NetCore.Clients;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lti.v2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.Profiles
{
    public class ToolConsumerProfileControllerShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ToolConsumerProfileControllerShould()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();

            // Set the current directory to the compiler output directory so that
            // the reference json is found
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }

        [Fact]
        public async void ReturnAToolConsumerProfile()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(LtiConstants.LtiToolConsumerProfileMediaType));

            using (var response = await _client.GetAsync("ims/toolconsumerprofile"))
            {
                response.EnsureSuccessStatusCode();
                Assert.Equal(LtiConstants.LtiToolConsumerProfileMediaType, response.Content.Headers.ContentType.MediaType);
                var profile = await response.Content.ReadJsonAsObjectAsync<ToolConsumerProfile>();
                Assert.NotNull(profile);
                JsonAssertions.AssertSameObjectJson(profile, "ToolConsumerProfile");
            }
        }

        [Fact]
        public async void ReturnAToolConsumerProfile_FromGetToolConsumerProfileAsync()
        {
            var clientResponse = await ToolConsumerProfileClient.GetToolConsumerProfileAsync(_client, "/ims/toolconsumerprofile");
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
            JsonAssertions.AssertSameObjectJson(clientResponse.Response, "ToolConsumerProfile");
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
