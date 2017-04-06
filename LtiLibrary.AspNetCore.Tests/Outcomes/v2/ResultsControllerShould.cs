using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v2
{
    public class ResultsControllerShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        private const string Url = "ims/results";
        private const string Key = "12345";
        private const string Secret = "secret";

        public ResultsControllerShould()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();

            // Set the current directory to the compiler output directory so that
            // the reference json is found
            Directory.SetCurrentDirectory(PlatformServices.Default.Application.ApplicationBasePath);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
