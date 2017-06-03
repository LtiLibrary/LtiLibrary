using System;
using System.IO;
using System.Net;
using System.Net.Http;
using LtiLibrary.AspNetCore.Tests.SimpleHelpers;
using LtiLibrary.NetCore.Clients;
using LtiLibrary.NetCore.Lis.v1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.Membership
{
    public class MembershipsControllerShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        private const string Key = "12345";
        private const string Secret = "secret";

        public MembershipsControllerShould()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();

            // Set the current directory to the compiler output directory so that
            // the reference json is found
            Directory.SetCurrentDirectory(PlatformServices.Default.Application.ApplicationBasePath);
        }

        [Fact]
        public async void GetAllMembershipPage_ReturnsNotFound_WhenOnGetMembershipIsNull()
        {
            // Given a working LTI Membership Service endpoint
            // When I call GetMembershipPageAsync
            var clientResponse = await MembershipClient.GetMembershipPageAsync(_client, "/api/memberships", Key, Secret);
            // Then I get a NotFound response
            Assert.Equal(HttpStatusCode.NotFound, clientResponse.StatusCode);
        }

        [Fact]
        public async void GetAllMembershipPage_ReturnsNotFound_WhenEndpointDoesNotExist()
        {
            // Given a non-existent LTI Membership Service endpoint
            // When I call GetMembershipPageAsync
            var clientResponse = await MembershipClient.GetMembershipPageAsync(_client, "/api/nowhere", Key, Secret);
            // Then I get a NotFound response
            Assert.Equal(HttpStatusCode.NotFound, clientResponse.StatusCode);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
