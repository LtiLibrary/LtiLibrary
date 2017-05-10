using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v1
{
    public class RawOutcomesControllerShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        private const string Url = "ims/rawoutcomes";
        private const string Key = "12345";
        private const string Secret = "secret";
        private const string Id = "testId";
        private const double Value = 0.5;

        public RawOutcomesControllerShould()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();

            // Set the current directory to the compiler output directory so that
            // the reference json is found
            Directory.SetCurrentDirectory(PlatformServices.Default.Application.ApplicationBasePath);
        }

        [Fact]
        public async Task Use_same_encoding_for_ContentType_and_Xml_for_DeleteResult()
        {
            var result = await Outcomes1Client.DeleteResultAsync(_client, Url, Key, Secret, Id);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Use_same_encoding_for_ContentType_and_Xml_for_ReadResult()
        {
            var result = await Outcomes1Client.ReadResultAsync(_client, Url, Key, Secret, Id);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Use_same_encoding_for_ContentType_and_Xml_for_ReplaceResult()
        {
            var result = await Outcomes1Client.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
