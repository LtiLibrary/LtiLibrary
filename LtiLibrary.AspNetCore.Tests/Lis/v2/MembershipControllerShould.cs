using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using LtiLibrary.AspNetCore.Tests.SimpleHelpers;
using Xunit;
using LtiLibrary.NetCore.Lis.v2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;

namespace LtiLibrary.AspNetCore.Tests.Lis.v2
{
    public class MembershipControllerShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        private const string Key = "12345";
        private const string Secret = "secret";

        public MembershipControllerShould()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();

            // Set the current directory to the compiler output directory so that
            // the reference json is found
            Directory.SetCurrentDirectory(PlatformServices.Default.Application.ApplicationBasePath);
        }

        [Fact]
        public async void GetsMembership()
        {
            var clientResponse = await MembershipClient.GetMembershipAsync(_client, "/ims/membership", Key, Secret);
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
            Assert.NotNull(clientResponse.Response);
            Assert.Equal(2, clientResponse.Response.Count());
        }

        [Fact]
        public async void GetsMembershipPage()
        {
            var clientResponse = await MembershipClient.GetMembershipPageAsync(_client, "/ims/membership", Key, Secret);
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
            Assert.NotNull(clientResponse.Response);
            JsonAssertions.AssertSameObjectJson(clientResponse.Response, "MembershipContainerPage");
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
