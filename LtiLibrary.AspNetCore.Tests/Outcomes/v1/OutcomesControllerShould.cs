using System;
using System.IO;
using System.Net;
using System.Net.Http;
using LtiLibrary.NetCore.Outcomes.v1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v1
{
    public class OutcomesControllerShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        private const string Url = "ims/outcomes";
        private const string Key = "12345";
        private const string Secret = "secret";
        private const string Id = "testId";
        private const double Value = 0.5;

        public OutcomesControllerShould()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();

            // Set the current directory to the compiler output directory so that
            // the reference json is found
            Directory.SetCurrentDirectory(PlatformServices.Default.Application.ApplicationBasePath);
        }

        [Fact]
        public async void ReplaceResult()
        {
            // TODO: Authentication!
            var readResult = await OutcomesClient.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            Assert.True(readResult.StatusCode == HttpStatusCode.OK, $"{readResult.StatusCode} == {HttpStatusCode.OK}");
        }

        [Fact]
        public async void ReadResult()
        {
            await OutcomesClient.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            var readResult = await OutcomesClient.ReadResultAsync(_client, Url, Key, Secret, Id);
            Assert.True(readResult.StatusCode == HttpStatusCode.OK, $"{readResult.StatusCode} == {HttpStatusCode.OK}");
            Assert.True(readResult.Response.Score == Value, $"{readResult.Response.Score} == {Value}");
            Assert.True(readResult.Response.SourcedId == Id, $"{readResult.Response.SourcedId} == {Id}");
        }

        [Fact]
        public async void DeleteResult()
        {
            await OutcomesClient.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            var deleteResult = await OutcomesClient.DeleteResultAsync(_client, Url, Key, Secret, Id);
            Assert.True(deleteResult.StatusCode == HttpStatusCode.OK, $"{deleteResult.StatusCode} == {HttpStatusCode.OK}");
            var readResult = await OutcomesClient.ReadResultAsync(_client, Url, Key, Secret, Id);
            Assert.True(readResult.Response == null, "readResult.Response == null");
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
