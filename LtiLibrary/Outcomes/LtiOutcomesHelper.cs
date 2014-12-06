using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using LtiLibrary.Common;
using LtiLibrary.Extensions;
using LtiLibrary.Models;
using LtiLibrary.OAuth;

namespace LtiLibrary.Outcomes
{
    public static class LtiOutcomesHelper
    {
        private static readonly XmlSerializer ImsxRequestSerializer;
        private static readonly XmlSerializer ImsxResponseSerializer;

        static LtiOutcomesHelper()
        {
            // The XSD code generator only creates one imsx_POXEnvelopeType which has the 
            // imsx_POXEnvelopeRequest root element. The IMS spec says the root element
            // should be imsx_POXEnvelopeResponse in the response.

            // Create two serializers: one for requests and one for responses.
            ImsxRequestSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType));
            ImsxResponseSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType), null, null, new XmlRootAttribute("imsx_POXEnvelopeResponse"), 
                    "http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0");
        }

        public static bool DeleteScore(string serviceUrl, string consumerKey, string consumerSecret, string lisResultSourcedId)
        {
            var imsxEnvelope = new imsx_POXEnvelopeType();
            imsxEnvelope.imsx_POXHeader = new imsx_POXHeaderType();
            imsxEnvelope.imsx_POXHeader.Item = new imsx_RequestHeaderInfoType();
            imsxEnvelope.imsx_POXBody = new imsx_POXBodyType();
            imsxEnvelope.imsx_POXBody.Item = new deleteResultRequest();

            var imsxHeader = imsxEnvelope.imsx_POXHeader.Item as imsx_RequestHeaderInfoType;
            imsxHeader.imsx_version = imsx_GWSVersionValueType.V10;
            imsxHeader.imsx_messageIdentifier = Guid.NewGuid().ToString();

            var imsxBody = imsxEnvelope.imsx_POXBody.Item as deleteResultRequest;
            imsxBody.resultRecord = new ResultRecordType();
            imsxBody.resultRecord.sourcedGUID = new SourcedGUIDType();
            imsxBody.resultRecord.sourcedGUID.sourcedId = lisResultSourcedId;

            try
            {
                var webRequest = CreateLtiOutcomesRequest(
                    imsxEnvelope,
                    serviceUrl,
                    consumerKey,
                    consumerSecret);
                var webResponse = webRequest.GetResponse() as HttpWebResponse;
                return ParseDeleteResultResponse(webResponse);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool PostScore(string serviceUrl, string consumerKey, string consumerSecret, string lisResultSourcedId, double? score)
        {
            var imsxEnvelope = new imsx_POXEnvelopeType();
            imsxEnvelope.imsx_POXHeader = new imsx_POXHeaderType();
            imsxEnvelope.imsx_POXHeader.Item = new imsx_RequestHeaderInfoType();
            imsxEnvelope.imsx_POXBody = new imsx_POXBodyType();
            imsxEnvelope.imsx_POXBody.Item = new replaceResultRequest();

            var imsxHeader = imsxEnvelope.imsx_POXHeader.Item as imsx_RequestHeaderInfoType;
            imsxHeader.imsx_version = imsx_GWSVersionValueType.V10;
            imsxHeader.imsx_messageIdentifier = Guid.NewGuid().ToString();

            var imsxBody = imsxEnvelope.imsx_POXBody.Item as replaceResultRequest;
            imsxBody.resultRecord = new ResultRecordType();
            imsxBody.resultRecord.sourcedGUID = new SourcedGUIDType();
            imsxBody.resultRecord.sourcedGUID.sourcedId = lisResultSourcedId;
            imsxBody.resultRecord.result = new ResultType();
            imsxBody.resultRecord.result.resultScore = new TextType();
            // The LTI 1.1 specification states in 6.1.1. that the score in replaceResult should
            // always be formatted using “en” formatting
            // (http://www.imsglobal.org/LTI/v1p1p1/ltiIMGv1p1p1.html#_Toc330273034).
            imsxBody.resultRecord.result.resultScore.language = LtiConstants.ScoreLanguage;
            imsxBody.resultRecord.result.resultScore.textString = score.HasValue
                ? score.Value.ToString(CultureInfo.CreateSpecificCulture(LtiConstants.ScoreLanguage))
                : null;

            try
            {
                var webRequest = CreateLtiOutcomesRequest(
                    imsxEnvelope,
                    serviceUrl,
                    consumerKey,
                    consumerSecret);
                var webResponse = webRequest.GetResponse() as HttpWebResponse;
                return ParsePostResultResponse(webResponse);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool ParseDeleteResultResponse(HttpWebResponse webResponse)
        {
            if (webResponse == null) return false;

            var stream = webResponse.GetResponseStream();
            if (stream == null) return false;

            var imsxEnvelope = ImsxResponseSerializer.Deserialize(stream) as imsx_POXEnvelopeType;
            if (imsxEnvelope == null) return false;

            var imsxHeader = imsxEnvelope.imsx_POXHeader.Item as imsx_ResponseHeaderInfoType;
            if (imsxHeader == null) return false;

            var imsxStatus = imsxHeader.imsx_statusInfo.imsx_codeMajor;

            return imsxStatus == imsx_CodeMajorType.success;
        }

        private static bool ParsePostResultResponse(HttpWebResponse webResponse)
        {
            if (webResponse == null) return false;

            var stream = webResponse.GetResponseStream();
            if (stream == null) return false;

            var imsxEnvelope = ImsxResponseSerializer.Deserialize(stream) as imsx_POXEnvelopeType;
            if (imsxEnvelope == null) return false;

            var imsxHeader = imsxEnvelope.imsx_POXHeader.Item as imsx_ResponseHeaderInfoType;
            if (imsxHeader == null) return false;

            var imsxStatus = imsxHeader.imsx_statusInfo.imsx_codeMajor;

            return imsxStatus == imsx_CodeMajorType.success;
        }

        public static LisResult ReadScore(string serviceUrl, string consumerKey, string consumerSecret, string lisResultSourcedId)
        {
            var imsxEnvelope = new imsx_POXEnvelopeType();
            imsxEnvelope.imsx_POXHeader = new imsx_POXHeaderType();
            imsxEnvelope.imsx_POXHeader.Item = new imsx_RequestHeaderInfoType();
            imsxEnvelope.imsx_POXBody = new imsx_POXBodyType();
            imsxEnvelope.imsx_POXBody.Item = new readResultRequest();

            var imsxHeader = imsxEnvelope.imsx_POXHeader.Item as imsx_RequestHeaderInfoType;
            imsxHeader.imsx_version = imsx_GWSVersionValueType.V10;
            imsxHeader.imsx_messageIdentifier = Guid.NewGuid().ToString();

            var imsxBody = imsxEnvelope.imsx_POXBody.Item as readResultRequest;
            imsxBody.resultRecord = new ResultRecordType();
            imsxBody.resultRecord.sourcedGUID = new SourcedGUIDType();
            imsxBody.resultRecord.sourcedGUID.sourcedId = lisResultSourcedId;

            try
            {
                var webRequest = CreateLtiOutcomesRequest(
                    imsxEnvelope,
                    serviceUrl,
                    consumerKey,
                    consumerSecret);
                var webResponse = webRequest.GetResponse() as HttpWebResponse;
                return ParseReadResultResponse(webResponse);
            }
            catch
            {
                return new LisResult {IsValid = false};
            }
        }

        private static LisResult ParseReadResultResponse(WebResponse webResponse)
        {
            if (webResponse == null)
            {
                return new LisResult { IsValid = false };
            }

            var stream = webResponse.GetResponseStream();
            if (stream == null)
            {
                return new LisResult { IsValid = false };
            }

            var imsxEnvelope = (imsx_POXEnvelopeType)ImsxResponseSerializer.Deserialize(stream);
            var imsxHeader = (imsx_ResponseHeaderInfoType) imsxEnvelope.imsx_POXHeader.Item;
            var imsxStatus = imsxHeader.imsx_statusInfo.imsx_codeMajor;

            if (imsxStatus != imsx_CodeMajorType.success)
            {
                return new LisResult { IsValid = false };
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

        private static HttpWebRequest CreateLtiOutcomesRequest(imsx_POXEnvelopeType imsxEnvelope, string url, string consumerKey, string consumerSecret)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/xml";

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
            using (var ms = new MemoryStream())
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                ImsxRequestSerializer.Serialize(ms, imsxEnvelope);
                ms.Position = 0;
                ms.CopyTo(webRequest.GetRequestStream());

                var hash = sha1.ComputeHash(ms.ToArray());
                var hash64 = Convert.ToBase64String(hash);
                parameters.AddParameter(OAuthConstants.BodyHashParameter, hash64);
            }

            // Calculate the signature
            var signature = OAuthUtility.GenerateSignature(webRequest.Method, webRequest.RequestUri, parameters,
                consumerSecret);
            parameters.AddParameter(OAuthConstants.SignatureParameter, signature);

            // Build the Authorization header
            var authorization = new StringBuilder(OAuthConstants.AuthScheme).Append(" ");
            foreach (var key in parameters.AllKeys)
            {
                authorization.AppendFormat("{0}=\"{1}\",", key, HttpUtility.UrlEncode(parameters[key]));
            }
            webRequest.Headers["Authorization"] = authorization.ToString(0, authorization.Length - 1);

            return webRequest;
        }
    }
}