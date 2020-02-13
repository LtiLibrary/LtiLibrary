using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lti.v1;
using LtiLibrary.NetCore.OAuth;

namespace LtiLibrary.NetCore.Clients
{
    /// <summary>
    /// Helper methods for the Basic Outcomes service introduced in LTI 1.1.
    /// </summary>
    public static class Outcomes1Client
    {
        private static readonly XmlSerializer ImsxRequestSerializer;
        private static readonly XmlSerializer ImsxResponseSerializer;

        static Outcomes1Client()
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
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <returns>A <see cref="ClientResponse"/>.</returns>
        public static async Task<ClientResponse> DeleteResultAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret,
            string sourcedId, SignatureMethod signatureMethod = SignatureMethod.HmacSha1)
        {
            try
            {
                var imsxEnvelope = new imsx_POXEnvelopeType
                {
                    imsx_POXHeader = new imsx_POXHeaderType { Item = new imsx_RequestHeaderInfoType() },
                    imsx_POXBody = new imsx_POXBodyType { Item = new deleteResultRequest() }
                };

                var imsxHeader = (imsx_RequestHeaderInfoType)imsxEnvelope.imsx_POXHeader.Item;
                imsxHeader.imsx_version = imsx_GWSVersionValueType.V10;
                imsxHeader.imsx_messageIdentifier = Guid.NewGuid().ToString();

                var imsxBody = (deleteResultRequest)imsxEnvelope.imsx_POXBody.Item;
                imsxBody.resultRecord = new ResultRecordType
                {
                    sourcedGUID = new SourcedGUIDType { sourcedId = sourcedId }
                };

                var outcomeResponse = new ClientResponse();
                try
                {
                    // Create a UTF8 encoding of the request
                    var xml = await GetXmlAsync(imsxEnvelope).ConfigureAwait(false);
                    var xmlContent = new StringContent(xml, Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType);
                    HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Post, serviceUrl)
                    {
                        Content = xmlContent
                    };
                    webRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(LtiConstants.ImsxOutcomeMediaType));
                    await SecuredClient.SignRequest(client, webRequest, consumerKey, consumerSecret, signatureMethod)
                        .ConfigureAwait(false);

                    // Post the request and check the response
                    using (var response = await client.SendAsync(webRequest).ConfigureAwait(false))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.IsSuccessStatusCode)
                        {
                            var imsxResponseEnvelope = (imsx_POXEnvelopeType)ImsxResponseSerializer.Deserialize(
                                await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
                            var imsxResponseHeader = (imsx_ResponseHeaderInfoType)imsxResponseEnvelope.imsx_POXHeader.Item;
                            var imsxResponseStatus = imsxResponseHeader.imsx_statusInfo.imsx_codeMajor;

                            outcomeResponse.StatusCode = imsxResponseStatus == imsx_CodeMajorType.success
                                ? HttpStatusCode.OK
                                : HttpStatusCode.BadRequest;
                        }
#if DEBUG
                        outcomeResponse.HttpRequest = await response.RequestMessage.ToFormattedRequestStringAsync
                            (new StringContent(xml, Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType))
                            .ConfigureAwait(false);
#endif
                        outcomeResponse.HttpResponse = await response.ToFormattedResponseStringAsync()
                            .ConfigureAwait(false);
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
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <returns>A <see cref="ClientResponse"/>.</returns>
        public static async Task<ClientResponse<Result>> ReadResultAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret,
            string lisResultSourcedId, SignatureMethod signatureMethod = SignatureMethod.HmacSha1)
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

                var outcomeResponse = new ClientResponse<Result>();
                try
                {
                    // Create a UTF8 encoding of the request
                    var xml = await GetXmlAsync(imsxEnvelope).ConfigureAwait(false);
                    var xmlContent = new StringContent(xml, Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType);
                    HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Post, serviceUrl)
                    {
                        Content = xmlContent
                    };
                    webRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(LtiConstants.ImsxOutcomeMediaType));
                    await SecuredClient.SignRequest(client, webRequest, consumerKey, consumerSecret, signatureMethod)
                        .ConfigureAwait(false);

                    // Post the request and check the response
                    using (var response = await client.SendAsync(webRequest).ConfigureAwait(false))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.IsSuccessStatusCode)
                        {
                            var imsxResponseEnvelope = (imsx_POXEnvelopeType)ImsxResponseSerializer.Deserialize
                                (await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
                            var imsxResponseHeader = (imsx_ResponseHeaderInfoType)imsxResponseEnvelope.imsx_POXHeader.Item;
                            var imsxResponseStatus = imsxResponseHeader.imsx_statusInfo.imsx_codeMajor;

                            if (imsxResponseStatus == imsx_CodeMajorType.success)
                            {
                                var imsxResponseBody = (readResultResponse)imsxResponseEnvelope.imsx_POXBody.Item;
                                if (imsxResponseBody?.result == null)
                                {
                                    outcomeResponse.Response = new Result { Score = null };
                                }
                                else
                                {
                                    // The TP is supposed to use "en" language format, but this allows
                                    // a little bit of misbehaving. If the TP does not include a language, "en" will
                                    // be used. If the TP does include a language (even a non-en language), it will
                                    // be used.
                                    var cultureInfo = new CultureInfo(imsxResponseBody.result.resultScore.language ?? LtiConstants.ScoreLanguage);
                                    outcomeResponse.Response = double.TryParse(imsxResponseBody.result.resultScore.textString, NumberStyles.Number, cultureInfo, out var score)
                                        ? new Result { Score = score, SourcedId = lisResultSourcedId }
                                        : new Result { Score = null, SourcedId = lisResultSourcedId };

                                    // Optional Canvas-style submission details
                                    var resultData = imsxResponseBody.result.ResultData;
                                    outcomeResponse.Response.LtiLaunchUrl = resultData?.LtiLaunchUrl;
                                    outcomeResponse.Response.Text = resultData?.Text;
                                    outcomeResponse.Response.Url = resultData?.Url;
                                }
                            }
                            else
                            {
                                outcomeResponse.StatusCode = HttpStatusCode.BadRequest;
                            }
                        }
#if DEBUG
                        outcomeResponse.HttpRequest = await response.RequestMessage.ToFormattedRequestStringAsync
                            (new StringContent(xml, Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType))
                            .ConfigureAwait(false);
#endif
                        outcomeResponse.HttpResponse = await response.ToFormattedResponseStringAsync()
                            .ConfigureAwait(false);
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
                return new ClientResponse<Result>
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
        /// <param name="text">Optional text data (Canvas extension)</param>
        /// <param name="url">Optional url data</param>
        /// <param name="ltiLaunchUrl">Optional LTI launch URL data</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <returns>A <see cref="ClientResponse"/>.</returns>
        public static async Task<ClientResponse> ReplaceResultAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret,
            string lisResultSourcedId, double? score, string text = null, string url = null, string ltiLaunchUrl = null, SignatureMethod signatureMethod = SignatureMethod.HmacSha1)
        {
            try
            {
                var imsxEnvelope = new imsx_POXEnvelopeType
                {
                    imsx_POXHeader = new imsx_POXHeaderType { Item = new imsx_RequestHeaderInfoType() },
                    imsx_POXBody = new imsx_POXBodyType { Item = new replaceResultRequest() }
                };

                var imsxHeader = (imsx_RequestHeaderInfoType)imsxEnvelope.imsx_POXHeader.Item;
                imsxHeader.imsx_version = imsx_GWSVersionValueType.V10;
                imsxHeader.imsx_messageIdentifier = Guid.NewGuid().ToString();

                var imsxBody = (replaceResultRequest)imsxEnvelope.imsx_POXBody.Item;
                imsxBody.resultRecord = new ResultRecordType
                {
                    sourcedGUID = new SourcedGUIDType { sourcedId = lisResultSourcedId },
                    result = new ResultType
                    {
                        resultScore = new TextType
                        {
                            language = LtiConstants.ScoreLanguage,
                            // The LTI 1.1 specification states in 6.1.1. that the score in replaceResult should
                            // always be formatted using “en” formatting
                            // (http://www.imsglobal.org/LTI/v1p1p1/ltiIMGv1p1p1.html#_Toc330273034).
                            textString = score?.ToString(new CultureInfo(LtiConstants.ScoreLanguage))
                        }
                    }
                };

                // If any ResultData is supplied, add the ResultData element
                if (!string.IsNullOrEmpty(text + url + ltiLaunchUrl))
                {
                    var resultData = imsxBody.resultRecord.result.ResultData = new DataType();
                    resultData.LtiLaunchUrl = ltiLaunchUrl;
                    resultData.Text = text;
                    resultData.Url = url;
                }

                var outcomeResponse = new ClientResponse();
                try
                {
                    // Create a UTF8 encoding of the request
                    var xml = await GetXmlAsync(imsxEnvelope).ConfigureAwait(false);
                    var xmlContent = new StringContent(xml, Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType);
                    HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Post, serviceUrl)
                    {
                        Content = xmlContent
                    };
                    webRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(LtiConstants.ImsxOutcomeMediaType));
                    await SecuredClient.SignRequest(client, webRequest, consumerKey, consumerSecret, signatureMethod)
                        .ConfigureAwait(false);

                    // Post the request and check the response
                    using (var response = await client.SendAsync(webRequest).ConfigureAwait(false))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.IsSuccessStatusCode)
                        {
                            var imsxResponseEnvelope = (imsx_POXEnvelopeType)ImsxResponseSerializer.Deserialize
                                (await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
                            var imsxResponseHeader = (imsx_ResponseHeaderInfoType)imsxResponseEnvelope.imsx_POXHeader.Item;
                            var imsxResponseStatus = imsxResponseHeader.imsx_statusInfo.imsx_codeMajor;

                            outcomeResponse.StatusCode = imsxResponseStatus == imsx_CodeMajorType.success
                                ? HttpStatusCode.OK
                                : HttpStatusCode.BadRequest;
                        }
#if DEBUG
                        outcomeResponse.HttpRequest = await response.RequestMessage.ToFormattedRequestStringAsync
                            (new StringContent(xml, Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType))
                            .ConfigureAwait(false);
#endif
                        outcomeResponse.HttpResponse = await response.ToFormattedResponseStringAsync()
                            .ConfigureAwait(false);
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
        /// Serialize the <see cref="imsx_POXEnvelopeType"/> into a <see cref="Encoding.UTF8"/> encoded string.
        /// </summary>
        /// <param name="imsxEnvelope"></param>
        /// <returns></returns>
        private static async Task<string> GetXmlAsync(imsx_POXEnvelopeType imsxEnvelope)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer =
                    XmlWriter.Create(ms, new XmlWriterSettings
                    {
                        Async = true,
                        Encoding = new UTF8Encoding(false),
                        Indent = true
                    }))
                {
                    ImsxRequestSerializer.Serialize(writer, imsxEnvelope);
                    await writer.FlushAsync();
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
