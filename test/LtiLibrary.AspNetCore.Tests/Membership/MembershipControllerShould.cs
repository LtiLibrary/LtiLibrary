﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using LtiLibrary.AspNetCore.Tests.SimpleHelpers;
using LtiLibrary.NetCore.Clients;
using LtiLibrary.NetCore.Lis.v1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.Membership
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
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }

        [Fact]
        public async void ReturnMembershipPage()
        {
            // Given a working LTI Membership Service endpoint
            // When I call GetMembershipPageAsync without any filters
            var clientResponse = await MembershipClient.GetMembershipPageAsync(_client, "/ims/membership/context/context-1", Key, Secret);
            // Then I get an OK response
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
            // And the response is not null
            Assert.NotNull(clientResponse.Response);
            // And the response matches the response in MembershipContainerPage.json
            JsonAssertions.AssertSameObjectJson(clientResponse.Response, "MembershipContainerPage");
        }

        [Fact]
        public async void ReturnNotFound_WhenTheSpecifiedPageDoesNotExist()
        {
            // Given a working LTI Membership Service endpoint
            // When I call GetMembershipPageAsync with an invalid page number
            var clientResponse = await MembershipClient.GetMembershipPageAsync(_client, "/ims/membership/context/context-1?page=3", Key, Secret);
            // Then I get a NotFound response
            Assert.Equal(HttpStatusCode.NotFound, clientResponse.StatusCode);
        }

        [Fact]
        public async void ReturnAllMemberships()
        {
            // Given a working LTI Membership Service endpoint
            // When I call GetMembershipAsync without any filters
            var clientResponse = await MembershipClient.GetMembershipAsync(_client, "/ims/membership/context/context-1", Key, Secret);
            // Then I get an OK response
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
            // And the response is not null
            Assert.NotNull(clientResponse.Response);
            // And the response matches the response in Memberships.json
            JsonAssertions.AssertSameObjectJson(new {clientResponse.Response}, "Memberships");
        }

        [Fact]
        public async void ReturnsLearners_WhenRoleFilterIsLearner()
        {
            // Given a working LTI Membership Service endpoint
            // When I call GetMembershipAsync with the Learner role filter
            var clientResponse = await MembershipClient.GetMembershipAsync(_client, "/ims/membership/context/context-1", Key, Secret, role: Role.Learner);
            // Then I get an OK response
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
            // And the response is not null
            Assert.NotNull(clientResponse.Response);
            // And there is exactly one membership
            Assert.Equal(1, clientResponse.Response.Count);
        }

        [Fact]
        public async void ReturnNotFound_WhenThereIsNoContextId()
        {
            // Given a working LTI Membership Service endpoint
            // When I do not specify a contextId
            var clientResponse = await MembershipClient.GetMembershipAsync(_client, "/ims/membership/context", Key, Secret);
            // Then I get a NotFound response
            Assert.Equal(HttpStatusCode.NotFound, clientResponse.StatusCode);
        }

        [Fact]
        public async void ReturnNotFound_WhenThereIsAnUnknownContextId()
        {
            // Given a working LTI Membership Service endpoint
            // When I specify an unknown contextId
            var clientResponse = await MembershipClient.GetMembershipAsync(_client, "/ims/membership/context/context-2", Key, Secret);
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
