using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Lti1;

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
                    await SignRequest(client, serviceUrl, httpContent, consumerKey, consumerSecret);
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
                    await SignRequest(client, serviceUrl, httpContent, consumerKey, consumerSecret);
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
                    await SignRequest(client, serviceUrl, httpContent, consumerKey, consumerSecret);
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

        private static async Task SignRequest(HttpClient client, string serviceUrl, HttpContent content, string consumerKey, string consumerSecret)
        {
            var ltiRequest = new LtiRequest(LtiConstants.OutcomesMessageType)
            {
                ConsumerKey = consumerKey,
                HttpMethod = HttpMethod.Post.Method,
                Url = new Uri(client.BaseAddress, serviceUrl)
            };

            AuthenticationHeaderValue authorizationHeader;

            // Create an Authorization header using the body hash
            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(await content.ReadAsByteArrayAsync());
                authorizationHeader = ltiRequest.GenerateAuthorizationHeader(hash, consumerSecret);
            }

            // Attach the header to the client request
            client.DefaultRequestHeaders.Authorization = authorizationHeader;
        }

        #endregion

    }
}