using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v2
{
    public class ResultsControllerShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ResultsControllerShould()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();

            // Set the current directory to the compiler output directory so that
            // the reference json is found
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
