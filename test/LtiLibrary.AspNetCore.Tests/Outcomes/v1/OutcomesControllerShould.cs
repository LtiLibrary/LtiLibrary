using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using LtiLibrary.NetCore.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v1
{
    public class OutcomesControllerShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        private const string Url = "basepath/ims/outcomes";
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
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }

        [Fact]
        public async void ReplaceResult_WhenCultureIsEN()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-EN");
            var replaceResult = await Outcomes1Client.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            Assert.Equal(HttpStatusCode.OK, replaceResult.StatusCode);

            var start = replaceResult.HttpResponse.IndexOf("<imsx_description>", StringComparison.Ordinal) + "<imsx_description>".Length;
            var end = replaceResult.HttpResponse.IndexOf("</imsx_description>", StringComparison.Ordinal);
            Assert.Equal("Score for testId is now 0.5", replaceResult.HttpResponse.Substring(start, end-start));
        }

        [Fact]
        public async void ReplaceResult_WhenCultureIsNL()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");
            var replaceResult = await Outcomes1Client.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            Assert.Equal(HttpStatusCode.OK, replaceResult.StatusCode);

            var start = replaceResult.HttpResponse.IndexOf("<imsx_description>", StringComparison.Ordinal) + "<imsx_description>".Length;
            var end = replaceResult.HttpResponse.IndexOf("</imsx_description>", StringComparison.Ordinal);
            Assert.Equal("Score for testId is now 0.5", replaceResult.HttpResponse.Substring(start, end - start));
        }

        [Fact]
        public async void ReadResult_WhenCultureIsEN()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-EN");
            await Outcomes1Client.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            var readResult = await Outcomes1Client.ReadResultAsync(_client, Url, Key, Secret, Id);
            Assert.True(readResult.StatusCode == HttpStatusCode.OK, $"{readResult.StatusCode} == {HttpStatusCode.OK}");
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            Assert.Equal(Value, readResult.Response.Score);
        }

        [Fact]
        public async void ReadResult_WhenCultureIsNL()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");
            await Outcomes1Client.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            var readResult = await Outcomes1Client.ReadResultAsync(_client, Url, Key, Secret, Id);
            Assert.True(readResult.StatusCode == HttpStatusCode.OK, $"{readResult.StatusCode} == {HttpStatusCode.OK}");
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            Assert.Equal(Value, readResult.Response.Score);
        }

        [Fact]
        public async void DeleteResult()
        {
            await Outcomes1Client.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            var deleteResult = await Outcomes1Client.DeleteResultAsync(_client, Url, Key, Secret, Id);
            Assert.True(deleteResult.StatusCode == HttpStatusCode.OK, $"{deleteResult.StatusCode} == {HttpStatusCode.OK}");
            var readResult = await Outcomes1Client.ReadResultAsync(_client, Url, Key, Secret, Id);
            Assert.True(readResult.Response == null, "readResult.Response == null");
        }

        [Fact]
        public async void NotReplaceResult_IfUsingDifferentSecrets()
        {
            // First verify that plumbing works if secret is correct
            var replaceResult = await Outcomes1Client.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            Assert.True(replaceResult.StatusCode == HttpStatusCode.OK, $"{replaceResult.StatusCode} == {HttpStatusCode.OK}");

            // Now change secret and look for unauthorized
            replaceResult = await Outcomes1Client.ReplaceResultAsync(_client, Url, Key, "nosecret", Id, Value);
            Assert.True(replaceResult.StatusCode == HttpStatusCode.Unauthorized, $"{replaceResult.StatusCode} == {HttpStatusCode.Unauthorized}");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(-.01)]
        [InlineData(1.01)]
        public async void NotReplaceResult_IfScoreIsInvalid(double? score)
        {
            var replaceResult = await Outcomes1Client.ReplaceResultAsync(_client, Url, Key, Secret, Id, score);
            Assert.True(replaceResult.StatusCode == HttpStatusCode.BadRequest, $"{replaceResult.StatusCode} == {HttpStatusCode.BadRequest}");
        }

        [Fact]
        public async void NotReadResult_IfUsingDifferentSecrets()
        {
            // First verify that plumbing works if secret is correct
            await Outcomes1Client.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            var readResult = await Outcomes1Client.ReadResultAsync(_client, Url, Key, Secret, Id);
            Assert.True(readResult.StatusCode == HttpStatusCode.OK, $"{readResult.StatusCode} == {HttpStatusCode.OK}");

            // Now change secret and look for unauthorized
            readResult = await Outcomes1Client.ReadResultAsync(_client, Url, Key, "nosecret", Id);
            Assert.True(readResult.StatusCode == HttpStatusCode.Unauthorized, $"{readResult.StatusCode} == {HttpStatusCode.Unauthorized}");
        }

        [Fact]
        public async void NotDeleteResult_IfUsingDifferentSecrets()
        {
            // First verify that plumbing works if secret is correct
            await Outcomes1Client.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            var deleteResult = await Outcomes1Client.DeleteResultAsync(_client, Url, Key, Secret, Id);
            Assert.True(deleteResult.StatusCode == HttpStatusCode.OK, $"{deleteResult.StatusCode} == {HttpStatusCode.OK}");

            // Now change secret and look for unauthorized
            await Outcomes1Client.ReplaceResultAsync(_client, Url, Key, Secret, Id, Value);
            deleteResult = await Outcomes1Client.DeleteResultAsync(_client, Url, Key, "nosecret", Id);
            Assert.True(deleteResult.StatusCode == HttpStatusCode.Unauthorized, $"{deleteResult.StatusCode} == {HttpStatusCode.Unauthorized}");
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
