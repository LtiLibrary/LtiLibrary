using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.OAuth;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace LtiLibrary.NetCore.Outcomes.v1
{
    /// <summary>
    /// Helper methods for the Basic Outcomes service introduced in LTI 1.1.
    /// </summary>
    public static class OutcomesClient
    {
        private static readonly XmlSerializer ImsxRequestSerializer;
        private static readonly XmlSerializer ImsxResponseSerializer;

        static OutcomesClient()
        {
            // The XSD code generator only creates one imsx_POXEnvelopeType which has the 
            // imsx_POXEnvelopeRequest root element. The IMS spec says the root element
            // should be imsx_POXEnvelopeResponse in the response.

            // Create two serializers: one for requests and one for responses.
            ImsxRequestSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType),
                null, null, new XmlRootAttribute("imsx_POXEnvelopeRequest"),
                    "http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0");
            ImsxResponseSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType), 
                null, null, new XmlRootAttribute("imsx_POXEnvelopeResponse"), 
                    "http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0");
        }

        /// <summary>
        /// Send an Outcomes 1.0 DeleteResult request.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The URL to send the request to.</param>
        /// <param name="consumerKey">The OAuth key to sign the request.</param>
        /// <param name="consumerSecret">The OAuth secret to sign the request.</param>
        /// <param name="sourcedId">The LisResultSourcedId to be deleted.</param>
        /// <returns>A <see cref="ClientResponse"/>.</returns>
        public static async Task<ClientResponse> DeleteResultAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, string sourcedId)
        {
            try
            {
                var imsxEnvelope = new imsx_POXEnvelopeType
                {
                    imsx_POXHeader = new imsx_POXHeaderType {Item = new imsx_RequestHeaderInfoType()},
                    imsx_POXBody = new imsx_POXBodyType {Item = new deleteResultRequest()}
                };

                var imsxHeader = (imsx_RequestHeaderInfoType) imsxEnvelope.imsx_POXHeader.Item;
                imsxHeader.imsx_version = imsx_GWSVersionValueType.V10;
                imsxHeader.imsx_messageIdentifier = Guid.NewGuid().ToString();

                var imsxBody = (deleteResultRequest) imsxEnvelope.imsx_POXBody.Item;
                imsxBody.resultRecord = new ResultRecordType
                {
                    sourcedGUID = new SourcedGUIDType {sourcedId = sourcedId}
                };

                var outcomeResponse = new ClientResponse();
                try
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(LtiConstants.ImsxOutcomeMediaType));
                    var xml = new StringBuilder();
                    using (var writer = new StringWriter(xml))
                    {
                        ImsxRequestSerializer.Serialize(writer, imsxEnvelope);
                        await writer.FlushAsync();
                    }
                    var httpContent = new StringContent(xml.ToString(), Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType);
                    await SignRequest(client, HttpMethod.Post, serviceUrl, httpContent, consumerKey, consumerSecret);
                    using (var response = await client.PostAsync(serviceUrl, httpContent))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.IsSuccessStatusCode)
                        {
                            var imsxResponseEnvelope = (imsx_POXEnvelopeType)ImsxResponseSerializer.Deserialize(await response.Content.ReadAsStreamAsync());
                            var imsxResponseHeader = (imsx_ResponseHeaderInfoType)imsxResponseEnvelope.imsx_POXHeader.Item;
                            var imsxResponseStatus = imsxResponseHeader.imsx_statusInfo.imsx_codeMajor;

                            outcomeResponse.StatusCode = imsxResponseStatus == imsx_CodeMajorType.success
                                ? HttpStatusCode.OK
                                : HttpStatusCode.BadRequest;
                        }
#if DEBUG
                        outcomeResponse.HttpRequest = await response.RequestMessage.ToFormattedRequestStringAsync();
                        outcomeResponse.HttpResponse = await response.ToFormattedResponseStringAsync();
#endif
                    }
                }
                catch (HttpRequestException ex)
                {
                    outcomeResponse.Exception = ex;
                    outcomeResponse.StatusCode = HttpStatusCode.BadRequest;
                }
                catch (Exception ex)
                {
                    outcomeResponse.Exception = ex;
                    outcomeResponse.StatusCode = HttpStatusCode.InternalServerError;
                }
                return outcomeResponse;
            }
            catch (Exception ex)
            {
                return new ClientResponse
                {
                    Exception = ex,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        /// <summary>
        /// Send an Outcomes 1.0 ReadScore request and return the LisResult.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The URL to send the request to.</param>
        /// <param name="consumerKey">The OAuth key to sign the request.</param>
        /// <param name="consumerSecret">The OAuth secret to sign the request.</param>
        /// <param name="lisResultSourcedId">The LisResult to read.</param>
        /// <returns>A <see cref="ClientResponse"/>.</returns>
        public static async Task<ClientResponse<LisResult>> ReadResultAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, string lisResultSourcedId)
        {
            try
            {
                var imsxEnvelope = new imsx_POXEnvelopeType
                {
                    imsx_POXHeader = new imsx_POXHeaderType { Item = new imsx_RequestHeaderInfoType() },
                    imsx_POXBody = new imsx_POXBodyType { Item = new readResultRequest() }
                };

                var imsxHeader = (imsx_RequestHeaderInfoType)imsxEnvelope.imsx_POXHeader.Item;
                imsxHeader.imsx_version = imsx_GWSVersionValueType.V10;
                imsxHeader.imsx_messageIdentifier = Guid.NewGuid().ToString();

                var imsxBody = (readResultRequest)imsxEnvelope.imsx_POXBody.Item;
                imsxBody.resultRecord = new ResultRecordType
                {
                    sourcedGUID = new SourcedGUIDType { sourcedId = lisResultSourcedId }
                };

                var outcomeResponse = new ClientResponse<LisResult>();
                try
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(LtiConstants.ImsxOutcomeMediaType));
                    var xml = new StringBuilder();
                    using (var writer = new StringWriter(xml))
                    {
                        ImsxRequestSerializer.Serialize(writer, imsxEnvelope);
                        await writer.FlushAsync();
                    }
                    var httpContent = new StringContent(xml.ToString(), Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType);
                    await SignRequest(client, HttpMethod.Post, serviceUrl, httpContent, consumerKey, consumerSecret);
                    using (var response = await client.PostAsync(serviceUrl, httpContent))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.IsSuccessStatusCode)
                        {
                            var imsxResponseEnvelope = (imsx_POXEnvelopeType)ImsxResponseSerializer.Deserialize(await response.Content.ReadAsStreamAsync());
                            var imsxResponseHeader = (imsx_ResponseHeaderInfoType)imsxResponseEnvelope.imsx_POXHeader.Item;
                            var imsxResponseStatus = imsxResponseHeader.imsx_statusInfo.imsx_codeMajor;

                            if (imsxResponseStatus == imsx_CodeMajorType.success)
                            {
                                var imsxResponseBody = (readResultResponse)imsxResponseEnvelope.imsx_POXBody.Item;
                                if (imsxResponseBody?.result == null)
                                {
                                    outcomeResponse.Response = new LisResult { Score = null };
                                }
                                else
                                {
                                    outcomeResponse.Response = double.TryParse(imsxResponseBody.result.resultScore.textString, out double result) 
                                        ? new LisResult { Score = result, SourcedId = lisResultSourcedId } 
                                        : new LisResult { Score = null, SourcedId = lisResultSourcedId };
                                }
                            }
                            else
                            {
                                outcomeResponse.StatusCode = HttpStatusCode.BadRequest;
                            }
                        }
    #if DEBUG
                        outcomeResponse.HttpRequest = await response.RequestMessage.ToFormattedRequestStringAsync(new StringContent(xml.ToString(), Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType));
                        outcomeResponse.HttpResponse = await response.ToFormattedResponseStringAsync();
    #endif
                    }
                }
                catch (HttpRequestException ex)
                {
                    outcomeResponse.Exception = ex;
                    outcomeResponse.StatusCode = HttpStatusCode.BadRequest;
                }
                catch (Exception ex)
                {
                    outcomeResponse.Exception = ex;
                    outcomeResponse.StatusCode = HttpStatusCode.InternalServerError;
                }
                return outcomeResponse;
            }
            catch (Exception ex)
            {
                return new ClientResponse<LisResult>
                {
                    Exception = ex,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        /// <summary>
        /// Send an Outcomes 1.0 ReplaceResult request.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The URL to send the request to.</param>
        /// <param name="consumerKey">The OAuth key to sign the request.</param>
        /// <param name="consumerSecret">The OAuth secret to sign the request.</param>
        /// <param name="lisResultSourcedId">The LisResult to receive the score.</param>
        /// <param name="score">The score.</param>
        /// <returns>A <see cref="ClientResponse"/>.</returns>
        public static async Task<ClientResponse> ReplaceResultAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, string lisResultSourcedId, double? score)
        {
            try
            {
                var imsxEnvelope = new imsx_POXEnvelopeType
                {
                    imsx_POXHeader = new imsx_POXHeaderType {Item = new imsx_RequestHeaderInfoType()},
                    imsx_POXBody = new imsx_POXBodyType {Item = new replaceResultRequest()}
                };

                var imsxHeader = (imsx_RequestHeaderInfoType) imsxEnvelope.imsx_POXHeader.Item;
                imsxHeader.imsx_version = imsx_GWSVersionValueType.V10;
                imsxHeader.imsx_messageIdentifier = Guid.NewGuid().ToString();

                var imsxBody = (replaceResultRequest) imsxEnvelope.imsx_POXBody.Item;
                imsxBody.resultRecord = new ResultRecordType
                {
                    sourcedGUID = new SourcedGUIDType {sourcedId = lisResultSourcedId},
                    result = new ResultType
                    {
                        resultScore = new TextType
                        {
                            language = LtiConstants.ScoreLanguage,
                            textString = score?.ToString(new CultureInfo(LtiConstants.ScoreLanguage))
                        }
                    }
                };
                // The LTI 1.1 specification states in 6.1.1. that the score in replaceResult should
                // always be formatted using “en” formatting
                // (http://www.imsglobal.org/LTI/v1p1p1/ltiIMGv1p1p1.html#_Toc330273034).

                var outcomeResponse = new ClientResponse();
                try
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(LtiConstants.ImsxOutcomeMediaType));
                    var xml = new StringBuilder();
                    using (var writer = new StringWriter(xml))
                    {
                        ImsxRequestSerializer.Serialize(writer, imsxEnvelope);
                        await writer.FlushAsync();
                    }
                    var httpContent = new StringContent(xml.ToString(), Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType);
                    await SignRequest(client, HttpMethod.Post, serviceUrl, httpContent, consumerKey, consumerSecret);
                    using (var response = await client.PostAsync(serviceUrl, httpContent))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.IsSuccessStatusCode)
                        {
                            var imsxResponseEnvelope = (imsx_POXEnvelopeType)ImsxResponseSerializer.Deserialize(await response.Content.ReadAsStreamAsync());
                            var imsxResponseHeader = (imsx_ResponseHeaderInfoType)imsxResponseEnvelope.imsx_POXHeader.Item;
                            var imsxResponseStatus = imsxResponseHeader.imsx_statusInfo.imsx_codeMajor;

                            outcomeResponse.StatusCode = imsxResponseStatus == imsx_CodeMajorType.success
                                ? HttpStatusCode.OK
                                : HttpStatusCode.BadRequest;
                        }
#if DEBUG
                        outcomeResponse.HttpRequest = await response.RequestMessage.ToFormattedRequestStringAsync(new StringContent(xml.ToString(), Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType));
                        outcomeResponse.HttpResponse = await response.ToFormattedResponseStringAsync();
#endif
                    }
                }
                catch (HttpRequestException ex)
                {
                    outcomeResponse.Exception = ex;
                    outcomeResponse.StatusCode = HttpStatusCode.BadRequest;
                }
                catch (Exception ex)
                {
                    outcomeResponse.Exception = ex;
                    outcomeResponse.StatusCode = HttpStatusCode.InternalServerError;
                }
                return outcomeResponse;
            }
            catch (Exception ex)
            {
                return new ClientResponse
                {
                    Exception = ex,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        #region Private Methods

        private static async Task SignRequest(HttpClient client, HttpMethod method, string serviceUrl, HttpContent content, string consumerKey, string consumerSecret)
        {
            var parameters = new NameValueCollection();
            parameters.AddParameter(OAuthConstants.ConsumerKeyParameter, consumerKey);
            parameters.AddParameter(OAuthConstants.NonceParameter, Guid.NewGuid().ToString());
            parameters.AddParameter(OAuthConstants.SignatureMethodParameter, OAuthConstants.SignatureMethodHmacSha1);
            parameters.AddParameter(OAuthConstants.VersionParameter, OAuthConstants.Version10);

            // Calculate the timestamp
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timestamp = Convert.ToInt64(ts.TotalSeconds);
            parameters.AddParameter(OAuthConstants.TimestampParameter, timestamp);

            // Calculate the body hash
            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(await content.ReadAsByteArrayAsync());
                var hash64 = Convert.ToBase64String(hash);
                parameters.AddParameter(OAuthConstants.BodyHashParameter, hash64);
            }

            // Calculate the signature
            var signature = OAuthUtility.GenerateSignature(method.Method, new Uri(client.BaseAddress, serviceUrl), parameters, consumerSecret);
            parameters.AddParameter(OAuthConstants.SignatureParameter, signature);

            // Build the Authorization header
            var authorization = new StringBuilder(OAuthConstants.AuthScheme).Append(" ");
            foreach (var key in parameters.AllKeys)
            {
                authorization.AppendFormat("{0}=\"{1}\",", key, WebUtility.UrlEncode(parameters[key]));
            }
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authorization.ToString(0, authorization.Length - 1));
        }

        #endregion

    }
}