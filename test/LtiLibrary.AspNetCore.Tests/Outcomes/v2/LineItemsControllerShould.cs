using System;
using System.IO;
using System.Net;
using System.Net.Http;
using LtiLibrary.NetCore.Lis.v2;
using LtiLibrary.NetCore.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
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
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }

        [Fact]
        public async void ReturnNotFound_IfThereIsNoMatchingLineItem()
        {
            _fixture.InitializeData();

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems/{OutcomesDataFixture.LineItemId}";
            var clientResponse = await Outcomes2Client.GetLineItemAsync(_client, url, Key, Secret);
            Assert.Equal(HttpStatusCode.NotFound, clientResponse.StatusCode);
        }

        [Fact]
        public async void ReturnNotFound_IfThereAreNoMatchingLineItems()
        {
            _fixture.InitializeData();

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var clientResponse = await Outcomes2Client.GetLineItemsAsync(_client, url, Key, Secret, limit: 10);
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
        }

        [Fact]
        public async void ReturnLineItem_WhenValidLineItemIsPosted()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem {LineItemOf = new Context {ContextId = OutcomesDataFixture.ContextId }};

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var clientResponse = await Outcomes2Client.PostLineItemAsync(_client, url, Key, Secret, lineitem);
            Assert.Equal(HttpStatusCode.Created, clientResponse.StatusCode);
            Assert.NotNull(clientResponse.Response.Id);
            Assert.Equal(OutcomesDataFixture.ContextId, clientResponse.Response.LineItemOf.ContextId);
        }

        [Fact]
        public async void ReturnLineItem_WhenValidLineItemIsRequested()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var clientResponse = await Outcomes2Client.PostLineItemAsync(_client, url, Key, Secret, lineitem);

            url = clientResponse.Response.Id.AbsoluteUri;
            clientResponse = await Outcomes2Client.GetLineItemAsync(_client, url, Key, Secret);
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
            Assert.Equal(url, clientResponse.Response.Id.AbsoluteUri);
            Assert.Equal(OutcomesDataFixture.ContextId, clientResponse.Response.LineItemOf.ContextId);
        }

        [Fact]
        public async void PostLineItem_WhenSecretIsCorrect()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var clientResponse = await Outcomes2Client.PostLineItemAsync(_client, url, Key, Secret, lineitem);
            Assert.Equal(HttpStatusCode.Created, clientResponse.StatusCode);
        }

        [Fact]
        public async void NotPostLineItem_WhenSecretIsIncorrect()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var clientResponse = await Outcomes2Client.PostLineItemAsync(_client, url, Key, "nosecret", lineitem);
            Assert.Equal(HttpStatusCode.Unauthorized, clientResponse.StatusCode);
        }

        [Fact]
        public async void NotPutLineItem_WhenLineItemDoesNotExist()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var clientResponse = await Outcomes2Client.PutLineItemAsync(_client, url, Key, Secret, lineitem);
            Assert.Equal(HttpStatusCode.NotFound, clientResponse.StatusCode);
        }

        [Fact]
        public async void PutLineItem_WhenSecretIsCorrect()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var postResponse = await Outcomes2Client.PostLineItemAsync(_client, url, Key, Secret, lineitem);
            lineitem = postResponse.Response;

            lineitem.Label = "Updated";
            var clientResponse = await Outcomes2Client.PutLineItemAsync(_client, lineitem.Id.AbsoluteUri, Key, Secret, lineitem);
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);

            var readResponse = await Outcomes2Client.GetLineItemAsync(_client, lineitem.Id.AbsoluteUri, Key, Secret);
            Assert.Equal(HttpStatusCode.OK, readResponse.StatusCode);
            Assert.Equal("Updated", readResponse.Response.Label);
        }

        [Fact]
        public async void NotPutLineItem_WhenSecretIsNotCorrect()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems/lineitem-1";
            var clientResponse = await Outcomes2Client.PutLineItemAsync(_client, url, Key, "nosecret", lineitem);
            Assert.Equal(HttpStatusCode.Unauthorized, clientResponse.StatusCode);
        }

        [Fact]
        public async void NotDeleteLineItem_WhenLineItemDoesNotExist()
        {
            _fixture.InitializeData();
            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems/nolineitem";
            var clientResponse = await Outcomes2Client.DeleteLineItemAsync(_client, url, Key, Secret);
            Assert.Equal(HttpStatusCode.NotFound, clientResponse.StatusCode);
        }

        [Fact]
        public async void NotDeleteLineItem_WhenSecretIsNotCorrect()
        {
            _fixture.InitializeData();
            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems/nolineitem";
            var clientResponse = await Outcomes2Client.DeleteLineItemAsync(_client, url, Key, "nosecret");
            Assert.Equal(HttpStatusCode.Unauthorized, clientResponse.StatusCode);
        }

        [Fact]
        public async void DeleteLineItem_WhenLineItemExists()
        {
            _fixture.InitializeData();
            var lineitem = new LineItem { LineItemOf = new Context { ContextId = OutcomesDataFixture.ContextId } };

            var url = $"/ims/courses/{OutcomesDataFixture.ContextId}/lineitems";
            var postResponse = await Outcomes2Client.PostLineItemAsync(_client, url, Key, Secret, lineitem);
            lineitem = postResponse.Response;

            var clientResponse = await Outcomes2Client.DeleteLineItemAsync(_client, lineitem.Id.AbsoluteUri, Key, Secret);
            Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
