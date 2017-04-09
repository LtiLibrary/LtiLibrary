using System;
using System.IO;
using System.Net;
using System.Net.Http;
using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v2
{
    public class LineItemsControllerShould : IDisposable, IClassFixture<OutcomesDataFixture>
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly OutcomesDataFixture _fixture;

        private const string Key = "12345";
        private const string Secret = "secret";

        public LineItemsControllerShould(OutcomesDataFixture fixture)
        {
            _fixture = fixture;
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();

            // Set the current directory to the compiler output directory so that
            // the reference json is found
            Directory.SetCurrentDirectory(PlatformServices.Default.Application.ApplicationBasePath);
        }

        [Fact]
        public async void ReturnNotFound_IfThereIsNoMatchingLineItem()
        {
            _fixture.InitializeData();

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems/{OutcomesDataFixture.LineItemId}";
            var clientResponse = await OutcomesClient.GetLineItemAsync(_client, url, Key, Secret);
            Assert.True(clientResponse.StatusCode == HttpStatusCode.NotFound, $"{clientResponse.StatusCode} == {HttpStatusCode.NotFound}");
        }

        [Fact]
        public async void ReturnNotFound_IfThereAreNoMatchingLineItems()
        {
            _fixture.InitializeData();

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var clientResponse = await OutcomesClient.GetLineItemsAsync(_client, url, Key, Secret, limit: 10);
            Assert.True(clientResponse.StatusCode == HttpStatusCode.OK, $"{clientResponse.StatusCode} == {HttpStatusCode.NotFound}");
        }

        [Fact]
        public async void ReturnLineItem_WhenValidLineItemIsPosted()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem {LineItemOf = new Context {ContextId = OutcomesDataFixture.ContextId }};

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var clientResponse = await OutcomesClient.PostLineItemAsync(_client, url, Key, Secret, lineitem);
            Assert.True(clientResponse.StatusCode == HttpStatusCode.Created, $"clientResponse.StatusCode == {HttpStatusCode.Created}");
            Assert.True(clientResponse.Response.Id != null, "clientResponse.Response.Id != null");
            Assert.True(clientResponse.Response.LineItemOf.ContextId == OutcomesDataFixture.ContextId, $"clientResponse.Response.LineItemOf.ContextId == {OutcomesDataFixture.ContextId}");
        }

        [Fact]
        public async void ReturnLineItem_WhenValidLineItemIsRequested()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var clientResponse = await OutcomesClient.PostLineItemAsync(_client, url, Key, Secret, lineitem);

            url = clientResponse.Response.Id.AbsoluteUri;
            clientResponse = await OutcomesClient.GetLineItemAsync(_client, url, Key, Secret);
            Assert.True(clientResponse.StatusCode == HttpStatusCode.OK, $"clientResponse.StatusCode == {HttpStatusCode.OK}");
            Assert.True(clientResponse.Response.Id.AbsoluteUri == url, $"clientResponse.Response.Id == {url}");
            Assert.True(clientResponse.Response.LineItemOf.ContextId == OutcomesDataFixture.ContextId, $"clientResponse.Response.LineItemOf.ContextId == {OutcomesDataFixture.ContextId}");
        }

        [Fact]
        public async void PostLineItem_WhenSecretIsCorrect()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var clientResponse = await OutcomesClient.PostLineItemAsync(_client, url, Key, Secret, lineitem);
            Assert.True(clientResponse.StatusCode == HttpStatusCode.Created, $"clientResponse.StatusCode == {HttpStatusCode.Created}");
        }

        [Fact]
        public async void NotPostLineItem_WhenSecretIsIncorrect()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var clientResponse = await OutcomesClient.PostLineItemAsync(_client, url, Key, "nosecret", lineitem);
            Assert.True(clientResponse.StatusCode == HttpStatusCode.Unauthorized, $"clientResponse.StatusCode == {HttpStatusCode.Unauthorized}");
        }

        [Fact]
        public async void NotPutLineItem_WhenLineItemDoesNotExist()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var clientResponse = await OutcomesClient.PutLineItemAsync(_client, url, Key, Secret, lineitem);
            Assert.True(clientResponse.StatusCode == HttpStatusCode.NotFound, $"clientResponse.StatusCode == {HttpStatusCode.NotFound}");
        }

        [Fact]
        public async void PutLineItem_WhenSecretIsCorrect()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var postResponse = await OutcomesClient.PostLineItemAsync(_client, url, Key, Secret, lineitem);
            lineitem = postResponse.Response;

            lineitem.Label = "Updated";
            var clientResponse = await OutcomesClient.PutLineItemAsync(_client, lineitem.Id.AbsoluteUri, Key, Secret, lineitem);
            Assert.True(clientResponse.StatusCode == HttpStatusCode.OK, $"clientResponse.StatusCode == {HttpStatusCode.OK}");

            var readResponse = await OutcomesClient.GetLineItemAsync(_client, lineitem.Id.AbsoluteUri, Key, Secret);
            Assert.True(readResponse.StatusCode == HttpStatusCode.OK, $"clientResponse.StatusCode == {HttpStatusCode.OK}");
            Assert.True(readResponse.Response.Label == "Updated", "LineItem.Label == \"Updated\"");
        }

        [Fact]
        public async void NotPutLineItem_WhenSecretIsNotCorrect()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems/lineitem-1";
            var clientResponse = await OutcomesClient.PutLineItemAsync(_client, url, Key, "nosecret", lineitem);
            Assert.True(clientResponse.StatusCode == HttpStatusCode.Unauthorized, $"{clientResponse.StatusCode} == {HttpStatusCode.Unauthorized}");
        }

        [Fact]
        public async void NotDeleteLineItem_WhenLineItemDoesNotExist()
        {
            _fixture.InitializeData();
            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems/nolineitem";
            var clientResponse = await OutcomesClient.DeleteLineItemAsync(_client, url, Key, Secret);
            Assert.True(clientResponse.StatusCode == HttpStatusCode.NotFound, $"{clientResponse.StatusCode} == {HttpStatusCode.NotFound}");
        }

        [Fact]
        public async void NotDeleteLineItem_WhenSecretIsNotCorrect()
        {
            _fixture.InitializeData();
            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems/nolineitem";
            var clientResponse = await OutcomesClient.DeleteLineItemAsync(_client, url, Key, "nosecret");
            Assert.True(clientResponse.StatusCode == HttpStatusCode.Unauthorized, $"{clientResponse.StatusCode} == {HttpStatusCode.Unauthorized}");
        }

        [Fact]
        public async void DeleteLineItem_WhenLineItemExists()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var postResponse = await OutcomesClient.PostLineItemAsync(_client, url, Key, Secret, lineitem);
            lineitem = postResponse.Response;

            var clientResponse = await OutcomesClient.DeleteLineItemAsync(_client, lineitem.Id.AbsoluteUri, Key, Secret);
            Assert.True(clientResponse.StatusCode == HttpStatusCode.OK, $"clientResponse.StatusCode == {HttpStatusCode.OK}");
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
