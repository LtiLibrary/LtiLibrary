using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Tests.SimpleHelpers;
using LtiLibrary.NetCore.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.Membership
{
    public class MembershipClientShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        private const string Key = "12345";
        private const string Secret = "secret";

        public MembershipClientShould()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();

            // Set the current directory to the compiler output directory so that
            // the reference json is found
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }

        [Fact]
        public async Task ReturnAllMemberships_WhenImageUrlsAreValid()
        {
            // Given a working LTI Membership Service endpoint
            // When I call GetMembershipAsync without any filters
            var clientResponse = await MembershipClient.GetMembershipAsync(_client, "/ims/rawmembership/RawMemberships", Key, Secret);
            // Then I get an OK response
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
            // And the response is not null
            Assert.NotNull(clientResponse.Response);
            // And the response matches the response in Memberships.json
            JsonAssertions.AssertSameObjectJson(new {clientResponse.Response}, "Memberships");
        }

        [Fact]
        public async Task ReturnNoMemberships_WhenImageUrlsAreInvalid_AndErrorHandlerIsNull()
        {
            // Given a working LTI Membership Service endpoint
            // When I call GetMembershipAsync without any filters
            var clientResponse = await MembershipClient.GetMembershipAsync(_client, "/ims/rawmembership/RawMembershipsWithInvalidImageUrl", Key, Secret);
            // Then I get an OK response
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
            // And the response is not null
            Assert.NotNull(clientResponse.Response);
            // And the response is empty
            Assert.Equal(0, clientResponse.Response.Count);
        }
        
        [Fact]
        public async Task ReturnAllMemberships_WhenImageUrlsAreInvalid_AndErrorHandlerIgnoresErrors()
        {
            // Given a working LTI Membership Service endpoint
            // When I call GetMembershipAsync without any filters
            var clientResponse = await MembershipClient.GetMembershipAsync
                (
                    _client, "/ims/rawmembership/RawMembershipsWithInvalidImageUrl", Key, Secret,
                    // Ignore deserialization errors
                    deserializationErrorHandler: HandleDeserializationError
                );
            // Then I get an OK response
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
            // And the response is not null
            Assert.NotNull(clientResponse.Response);
            // And the response matches the response in Memberships.json
            JsonAssertions.AssertSameObjectJson(new {clientResponse.Response}, "MembershipsWithNoImages");
        }
        
        // Ignore deserialization errors
        private static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            e.ErrorContext.Handled = true;
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
