using System;
using System.IO;
using System.Net;
using System.Net.Http;
using LtiLibrary.NetCore.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
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
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }

        [Fact]
        public async void ReturnNotFound_WhenControllerHasNotImplementedOnGetMembershipAsync()
        {
            // Given a working LTI Membership Service endpoint
            // When I call GetMembershipPageAsync
            var clientResponse = await MembershipClient.GetMembershipPageAsync(_client, "/api/memberships/context/context-1", Key, Secret);
            // Then I get a NotFound response
            Assert.Equal(HttpStatusCode.NotFound, clientResponse.StatusCode);
        }

        [Fact]
        public async void ReturnNotFound_WhenEndpointDoesNotExist()
        {
            // Given a non-existent LTI Membership Service endpoint
            // When I call GetMembershipPageAsync
            var clientResponse = await MembershipClient.GetMembershipPageAsync(_client, "/api/nowhere/context/context-1", Key, Secret);
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
