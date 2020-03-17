using System;
using System.Collections.Generic;
using System.Net.Http;
using LtiLibrary.NetCore.Clients;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lis.v1;
using LtiLibrary.NetCore.Lti.v1;
using LtiLibrary.NetCore.OAuth;
using LtiLibrary.NetCore.Tests.TestHelpers;
using Xunit;

namespace LtiLibrary.NetCore.Tests
{
    public class SecureClientShould
    {
        #region SignRequest Tests
        [Fact]
        public async System.Threading.Tasks.Task HeaderContainsSHA256_IfSignatureMethodParamSetToSHA256()
        {
            //arrange
            var httpClient = new HttpClient();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://www.test.com/membership") 
            {
                Content = new StringContent("", System.Text.Encoding.UTF8)
            };
            //act
            await SecuredClient.SignRequest(httpClient, requestMessage, "TestConsumerKey",
                "TestConsumerSecret", SignatureMethod.HmacSha256);

            //assert
            Assert.NotNull(httpClient);
            Assert.NotNull(httpClient.DefaultRequestHeaders);
            Assert.NotNull(httpClient.DefaultRequestHeaders.Authorization);
            var headervalues = httpClient.DefaultRequestHeaders.Authorization.Parameter;
            var headerValuesList = headervalues.Split(',');
            Assert.True(headervalues.Contains("oauth_signature_method=\"HMAC-SHA256\""));
        }
        #endregion
    }
}
