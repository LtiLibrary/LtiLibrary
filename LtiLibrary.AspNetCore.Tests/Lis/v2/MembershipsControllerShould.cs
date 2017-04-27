using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;
using LtiLibrary.NetCore.Lis.v2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;

namespace LtiLibrary.AspNetCore.Tests.Lis.v2
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
        public async void ReturnMemberships()
        {
            var clientResponse = await MembershipClient.GetMembershipsAsync(_client, "/ims/memberships", Key, Secret);
            Assert.True(clientResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(clientResponse.Response);
            Assert.NotNull(clientResponse.Response.MembershipContainer);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
