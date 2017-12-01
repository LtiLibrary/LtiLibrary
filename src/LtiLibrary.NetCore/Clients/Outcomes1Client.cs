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
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="OAuthConstants.SignatureMethodHmacSha1"/>, currently <see cref="OAuthConstants.SignatureMethodHmacSha256"/> is also available</param>
        /// <returns>A <see cref="ClientResponse"/>.</returns>
        public static async Task<ClientResponse> DeleteResultAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, 
            string sourcedId, string signatureMethod = null)
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

                    // Create a UTF8 encoding of the request
                    var xml = await GetXmlAsync(imsxEnvelope);
                    var xmlContent = new StringContent(xml, Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType);
                    await SecuredClient.SignRequest(client, HttpMethod.Post, serviceUrl, xmlContent, consumerKey, consumerSecret, signatureMethod ?? OAuthConstants.SignatureMethodHmacSha1);

                    // Post the request and check the response
                    using (var response = await client.PostAsync(serviceUrl, xmlContent))
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
                        outcomeResponse.HttpRequest = await response.RequestMessage.ToFormattedRequestStringAsync(new StringContent(xml, Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType));
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
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="OAuthConstants.SignatureMethodHmacSha1"/>, currently <see cref="OAuthConstants.SignatureMethodHmacSha256"/> is also available</param>
        /// <returns>A <see cref="ClientResponse"/>.</returns>
        public static async Task<ClientResponse<Result>> ReadResultAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, 
            string lisResultSourcedId, string signatureMethod = null)
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
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(LtiConstants.ImsxOutcomeMediaType));

                    // Create a UTF8 encoding of the request
                    var xml = await GetXmlAsync(imsxEnvelope);
                    var xmlContent = new StringContent(xml, Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType);
                    await SecuredClient.SignRequest(client, HttpMethod.Post, serviceUrl, xmlContent, consumerKey, consumerSecret, signatureMethod ?? OAuthConstants.SignatureMethodHmacSha1);

                    // Post the request and check the response
                    using (var response = await client.PostAsync(serviceUrl, xmlContent))
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
                                    outcomeResponse.Response = new Result { Score = null };
                                }
                                else
                                {
                                    outcomeResponse.Response = double.TryParse(imsxResponseBody.result.resultScore.textString, out var result) 
                                        ? new Result { Score = result, SourcedId = lisResultSourcedId } 
                                        : new Result { Score = null, SourcedId = lisResultSourcedId };
                                }
                            }
                            else
                            {
                                outcomeResponse.StatusCode = HttpStatusCode.BadRequest;
                            }
                        }
    #if DEBUG
                        outcomeResponse.HttpRequest = await response.RequestMessage.ToFormattedRequestStringAsync(new StringContent(xml, Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType));
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
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="OAuthConstants.SignatureMethodHmacSha1"/>, currently <see cref="OAuthConstants.SignatureMethodHmacSha256"/> is also available</param>
        /// <returns>A <see cref="ClientResponse"/>.</returns>
        public static async Task<ClientResponse> ReplaceResultAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, 
            string lisResultSourcedId, double? score, string signatureMethod = null)
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
                            // The LTI 1.1 specification states in 6.1.1. that the score in replaceResult should
                            // always be formatted using “en” formatting
                            // (http://www.imsglobal.org/LTI/v1p1p1/ltiIMGv1p1p1.html#_Toc330273034).
                            textString = score?.ToString(new CultureInfo(LtiConstants.ScoreLanguage))
                        }
                    }
                };

                var outcomeResponse = new ClientResponse();
                try
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(LtiConstants.ImsxOutcomeMediaType));

                    // Create a UTF8 encoding of the request
                    var xml = await GetXmlAsync(imsxEnvelope);
                    var xmlContent = new StringContent(xml, Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType);
                    await SecuredClient.SignRequest(client, HttpMethod.Post, serviceUrl, xmlContent, consumerKey, consumerSecret, signatureMethod ?? OAuthConstants.SignatureMethodHmacSha1);

                    // Post the request and check the response
                    using (var response = await client.PostAsync(serviceUrl, xmlContent))
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
                        outcomeResponse.HttpRequest = await response.RequestMessage.ToFormattedRequestStringAsync(new StringContent(xml, Encoding.UTF8, LtiConstants.ImsxOutcomeMediaType));
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
                        Encoding = Encoding.UTF8,
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