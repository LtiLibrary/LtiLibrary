using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Extensions;
using LtiLibrary.Core.OAuth;
using System.Net.Http;
using System.Threading.Tasks;

namespace LtiLibrary.Core.Outcomes.v1
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

        public static async Task<BasicResult> DeleteScore(string serviceUrl, string consumerKey, string consumerSecret, string lisResultSourcedId)
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
                sourcedGUID = new SourcedGUIDType {sourcedId = lisResultSourcedId}
            };

            try
            {
                var webRequest = CreateLtiOutcomesRequest(
                    imsxEnvelope,
                    serviceUrl,
                    consumerKey,
                    consumerSecret);
                var webResponse = await webRequest.GetResponseAsync();
                return ParseDeleteResultResponse(webResponse);
            }
            catch (Exception ex)
            {
                return new BasicResult(false, ex.ToString());
            }
        }

        public static async Task<BasicResult> PostScore(string serviceUrl, string consumerKey, string consumerSecret, string lisResultSourcedId, double? score)
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
                        textString = score.HasValue
                            ? score.Value.ToString(new CultureInfo(LtiConstants.ScoreLanguage))
                            : null
                    }
                }
            };
            // The LTI 1.1 specification states in 6.1.1. that the score in replaceResult should
            // always be formatted using “en” formatting
            // (http://www.imsglobal.org/LTI/v1p1p1/ltiIMGv1p1p1.html#_Toc330273034).

            try
            {
                var webRequest = CreateLtiOutcomesRequest(
                    imsxEnvelope,
                    serviceUrl,
                    consumerKey,
                    consumerSecret);
                var webResponse = await webRequest.GetResponseAsync();
                return ParsePostResultResponse(webResponse);
            }
            catch (Exception ex)
            {
                return new BasicResult(false, ex.ToString());
            }
        }

        /// <summary>
        /// Returns True if the stream contains an LTI Outcomes payload.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> to examine.</param>
        /// <returns></returns>
        public static bool IsLtiOutcomesRequest(Stream stream)
        {
            imsx_POXEnvelopeType imsxEnvelope;
            try
            {
                imsxEnvelope = ImsxRequestSerializer.Deserialize(stream) as imsx_POXEnvelopeType;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
            }

            if (imsxEnvelope != null) return true;
            
            try
            {
                imsxEnvelope = ImsxResponseSerializer.Deserialize(stream) as imsx_POXEnvelopeType;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
            }

            return imsxEnvelope != null;
        }

        private static BasicResult ParseDeleteResultResponse(HttpResponseMessage webResponse)
        {
            if (webResponse == null) return new BasicResult(false, "Invalid webResponse");

            var stream = webResponse.GetResponseStream();
            if (stream == null) return new BasicResult(false, "Invalid stream");

            var imsxEnvelope = ImsxResponseSerializer.Deserialize(stream) as imsx_POXEnvelopeType;
            if (imsxEnvelope == null) return new BasicResult(false, "Invalid imsxEnvelope");

            var imsxHeader = imsxEnvelope.imsx_POXHeader.Item as imsx_ResponseHeaderInfoType;
            if (imsxHeader == null) return new BasicResult(false, "Invalid imsxHeader");

            var imsxStatus = imsxHeader.imsx_statusInfo.imsx_codeMajor;

            return new BasicResult(imsxStatus == imsx_CodeMajorType.success,
                imsxHeader.imsx_statusInfo.imsx_description);
        }

        private static BasicResult ParsePostResultResponse(HttpResponseMessage webResponse)
        {
            if (webResponse == null) return new BasicResult(false, "Invalid webResponse");

            var stream = webResponse.GetResponseStream();
            if (stream == null) return new BasicResult(false, "Invalid stream");

            var imsxEnvelope = ImsxResponseSerializer.Deserialize(stream) as imsx_POXEnvelopeType;
            if (imsxEnvelope == null) return new BasicResult(false, "Invalid imsxEnvelope");

            var imsxHeader = imsxEnvelope.imsx_POXHeader.Item as imsx_ResponseHeaderInfoType;
            if (imsxHeader == null) return new BasicResult(false, "Invalid imsxHeader");

            var imsxStatus = imsxHeader.imsx_statusInfo.imsx_codeMajor;

            return new BasicResult(imsxStatus == imsx_CodeMajorType.success,
                imsxHeader.imsx_statusInfo.imsx_description);
        }

        public static async Task<LisResult> ReadScore(string serviceUrl, string consumerKey, string consumerSecret, string lisResultSourcedId)
        {
            var imsxEnvelope = new imsx_POXEnvelopeType
            {
                imsx_POXHeader = new imsx_POXHeaderType {Item = new imsx_RequestHeaderInfoType()},
                imsx_POXBody = new imsx_POXBodyType {Item = new readResultRequest()}
            };

            var imsxHeader = (imsx_RequestHeaderInfoType) imsxEnvelope.imsx_POXHeader.Item;
            imsxHeader.imsx_version = imsx_GWSVersionValueType.V10;
            imsxHeader.imsx_messageIdentifier = Guid.NewGuid().ToString();

            var imsxBody = (readResultRequest) imsxEnvelope.imsx_POXBody.Item;
            imsxBody.resultRecord = new ResultRecordType
            {
                sourcedGUID = new SourcedGUIDType {sourcedId = lisResultSourcedId}
            };

            try
            {
                var webRequest = CreateLtiOutcomesRequest(
                    imsxEnvelope,
                    serviceUrl,
                    consumerKey,
                    consumerSecret);
                var webResponse = await webRequest.GetResponseAsync();
                return ParseReadResultResponse(webResponse);
            }
            catch (Exception ex)
            {
                return new LisResult {IsValid = false, Message = ex.ToString()};
            }
        }

        private static LisResult ParseReadResultResponse(HttpResponseMessage webResponse)
        {
            if (webResponse == null)
            {
                return new LisResult { IsValid = false, Message = "Invalid webResponse" };
            }

            var stream = webResponse.GetResponseStream();
            if (stream == null)
            {
                return new LisResult { IsValid = false, Message = "Invalid stream" };
            }

            var imsxEnvelope = (imsx_POXEnvelopeType)ImsxResponseSerializer.Deserialize(stream);
            var imsxHeader = (imsx_ResponseHeaderInfoType) imsxEnvelope.imsx_POXHeader.Item;
            var imsxStatus = imsxHeader.imsx_statusInfo.imsx_codeMajor;

            if (imsxStatus != imsx_CodeMajorType.success)
            {
                return new LisResult { IsValid = false, Message = imsxHeader.imsx_statusInfo.imsx_description};
            }

            var imsxBody = (readResultResponse) imsxEnvelope.imsx_POXBody.Item;

            if (imsxBody == null || imsxBody.result == null)
            {
                return new LisResult { Score = null, IsValid = true };
            }

            double result;
            if (double.TryParse(imsxBody.result.resultScore.textString, out result))
            {
                return new LisResult { Score = result, IsValid = true };
            }
            return new LisResult { Score = null, IsValid = true };
        }

        private static HttpRequestMessage CreateLtiOutcomesRequest(imsx_POXEnvelopeType imsxEnvelope, string url, string consumerKey, string consumerSecret)
        {
            var webRequest = new HttpRequestMessage(HttpMethod.Post, url);

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
            var ms = new MemoryStream();
            using (var sha1 = PlatformSpecific.GetSha1Provider())
            {
                ImsxRequestSerializer.Serialize(ms, imsxEnvelope);
                ms.Position = 0;
                webRequest.Content = new StreamContent(ms);
                webRequest.Content.Headers.ContentType = HttpContentType.Xml;

                var hash = sha1.ComputeHash(ms.ToArray());
                var hash64 = Convert.ToBase64String(hash);
                parameters.AddParameter(OAuthConstants.BodyHashParameter, hash64);
            }

            // Calculate the signature
            var signature = OAuthUtility.GenerateSignature(webRequest.Method.Method.ToUpper(), webRequest.RequestUri, parameters,
                consumerSecret);
            parameters.AddParameter(OAuthConstants.SignatureParameter, signature);

            // Build the Authorization header
            var authorization = new StringBuilder(OAuthConstants.AuthScheme).Append(" ");
            foreach (var key in parameters.AllKeys)
            {
                authorization.AppendFormat("{0}=\"{1}\",", key, WebUtility.UrlEncode(parameters[key]));
            }
            webRequest.Headers.Add(OAuthConstants.AuthorizationHeader, authorization.ToString(0, authorization.Length - 1));

            return webRequest;
        }
    }
}