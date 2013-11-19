using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using LtiLibrary.Models;
using OAuth.Net.Common;
using OAuth.Net.Components;

namespace LtiLibrary.Provider
{
    public static class LtiOutcomesHandler
    {
        private static readonly XmlSerializer ImsxSerializer;

        static LtiOutcomesHandler()
        {
            ImsxSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType));
        }

        public static bool PostScore(int outcomeId, double? score)
        {
            using (var db = new LtiLibraryContext())
            {
                var outcome = db.Outcomes.Find(outcomeId);
                if (outcome == null) return false;

                var consumer = db.Consumers.Find(outcome.ConsumerId);
                if (consumer == null) return false;

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
                imsxBody.resultRecord.sourcedGUID.sourcedId = outcome.LisResultSourcedId;
                imsxBody.resultRecord.result = new ResultType();
                imsxBody.resultRecord.result.resultScore = new TextType();
                imsxBody.resultRecord.result.resultScore.language = CultureInfo.CurrentCulture.Name;
                imsxBody.resultRecord.result.resultScore.textString = score.ToString();

                var webRequest = CreateLtiOutcomesRequest(
                    imsxEnvelope, 
                    outcome.ServiceUrl, 
                    consumer.Key, 
                    consumer.Secret);

                try
                {
                    var webResponse = webRequest.GetResponse() as HttpWebResponse;
                    return ParsePostResultResponse(webResponse);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private static bool ParsePostResultResponse(HttpWebResponse webResponse)
        {
            if (webResponse == null) return false;

            var stream = webResponse.GetResponseStream();
            if (stream == null) return false;

            var imsxEnvelope = ImsxSerializer.Deserialize(stream) as imsx_POXEnvelopeType;
            if (imsxEnvelope == null) return false;

            var imsxHeader = imsxEnvelope.imsx_POXHeader.Item as imsx_ResponseHeaderInfoType;
            if (imsxHeader == null) return false;

            var imsxStatus = imsxHeader.imsx_statusInfo.imsx_codeMajor;

            return imsxStatus == imsx_CodeMajorType.success;
        }

        public static LtiResult ReadScore(int outcomeId)
        {
            using (var db = new LtiLibraryContext())
            {
                var outcome = db.Outcomes.Find(outcomeId);
                if (outcome == null) return new LtiResult {IsValid = false};

                var consumer = db.Consumers.Find(outcome.ConsumerId);
                if (consumer == null) return new LtiResult { IsValid = false };

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
                imsxBody.resultRecord.sourcedGUID.sourcedId = outcome.LisResultSourcedId;

                var webRequest = CreateLtiOutcomesRequest(
                    imsxEnvelope, 
                    outcome.ServiceUrl, 
                    consumer.Key, 
                    consumer.Secret);

                try
                {
                    var webResponse = webRequest.GetResponse() as HttpWebResponse;
                    return ParseReadResultResponse(webResponse);
                }
                catch
                {
                    return new LtiResult {IsValid = false};
                }
            }
        }

        private static LtiResult ParseReadResultResponse(WebResponse webResponse)
        {
            if (webResponse == null)
            {
                return new LtiResult { IsValid = false };
            }

            var stream = webResponse.GetResponseStream();
            if (stream == null)
            {
                return new LtiResult { IsValid = false };
            }

            var imsxEnvelope = (imsx_POXEnvelopeType) ImsxSerializer.Deserialize(stream);
            var imsxHeader = (imsx_ResponseHeaderInfoType) imsxEnvelope.imsx_POXHeader.Item;
            var imsxStatus = imsxHeader.imsx_statusInfo.imsx_codeMajor;

            if (imsxStatus != imsx_CodeMajorType.success)
            {
                return new LtiResult { IsValid = false };
            }

            var imsxBody = (readResultResponse) imsxEnvelope.imsx_POXBody.Item;

            if (imsxBody == null || imsxBody.result == null)
            {
                return new LtiResult { Score = null, IsValid = true };
            }

            double result;
            if (double.TryParse(imsxBody.result.resultScore.textString, out result))
            {
                return new LtiResult { Score = result, IsValid = true };
            }
            return new LtiResult { Score = null, IsValid = true };
        }

        // Build the web request using OAuth.Net to build the Authorization header
        private static HttpWebRequest CreateLtiOutcomesRequest(imsx_POXEnvelopeType imsxEnvelope, string url, string consumerKey, string consumerSecret)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/xml";

            var parameters = new OAuthParameters();
            parameters.ConsumerKey = consumerKey;

            var signatureProvider = new HmacSha1SigningProvider();
            parameters.SignatureMethod = signatureProvider.SignatureMethod;

            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timestamp = Convert.ToInt32(ts.TotalSeconds);
            parameters.Timestamp = timestamp.ToString(CultureInfo.InvariantCulture);

            var nonceProvider = new GuidNonceProvider();
            parameters.Nonce = nonceProvider.GenerateNonce(timestamp);

            parameters.Version = "1.0";

            using (var ms = new MemoryStream())
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                ImsxSerializer.Serialize(ms, imsxEnvelope);
                ms.Position = 0;
                ms.CopyTo(webRequest.GetRequestStream());

                var hash = sha1.ComputeHash(ms.ToArray());
                var hash64 = Convert.ToBase64String(hash);
                parameters.AdditionalParameters.Add("oauth_body_hash", hash64);
            }

            var signatureBase = SignatureBase.Create(webRequest.Method, new Uri(url), parameters);
            parameters.Signature = signatureProvider.ComputeSignature(signatureBase, consumerSecret, string.Empty);

            var authorization = new StringBuilder(parameters.ToHeaderFormat());
            foreach (var key in parameters.AdditionalParameters.AllKeys)
            {
                authorization.AppendFormat(",{0}=\"{1}\"", key, HttpUtility.UrlEncode(parameters.AdditionalParameters[key]));
            }
            webRequest.Headers["Authorization"] = authorization.ToString();

            return webRequest;
        }
    }
}