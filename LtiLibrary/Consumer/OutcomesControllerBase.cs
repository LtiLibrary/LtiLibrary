using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;
using LtiLibrary.Models;
using OAuth.Net.Common;
using OAuth.Net.Components;
#if DEBUG
    using System.Diagnostics;
    using System.IO;
    using System.Xml.Serialization;
#endif

namespace LtiLibrary.Consumer
{
    /// <summary>
    /// Implements the LTI Basic Outcomes API.
    /// </summary>
    public abstract class OutcomesControllerBase : ApiController
    {
#if DEBUG
        static readonly XmlSerializer EnvelopeSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType));
#endif

        /// <summary>
        /// Return a list of OAuth ConsumerSecrets for the given ConsumerKey.
        /// These will be used to authenticate the request to the Basic Outcomes
        /// Service.
        /// </summary>
        /// <param name="consumerKey">The OAuth consumer key from the request.</param>
        /// <returns>A list of OAuth consumer secrets for the given consumer key.</returns>
        protected abstract IList<string> GetConsumerSecrets(string consumerKey);

        /// <summary>
        /// Delete the result (grade, score, outcome) from the consumer.
        /// </summary>
        /// <param name="result">The result to delete.</param>
        /// <returns>True if the result was deleted.</returns>
        protected abstract bool DeleteResult(Result result);
        /// <summary>
        /// Read the result (grade, score, outcome) from the consumer.
        /// </summary>
        /// <param name="result">The result the provider is looking for.</param>
        /// <returns>True if the result was read.</returns>
        protected abstract bool ReadResult(Result result);
        /// <summary>
        /// Save or update the result (grade, score, outcome) in the consumer.
        /// </summary>
        /// <param name="result">The result to save or update.</param>
        /// <returns>True if the result was saved or updated.</returns>
        protected abstract bool ReplaceResult(Result result);

        /// <summary>
        /// Authenticate and authorize the request.
        /// </summary>
        private void Authorize()
        {
            var request = ControllerContext.Request;

            if (request.Headers.Authorization == null)
            {
                OAuthRequestException.ThrowPermissionDenied("No authorization header");
            }

            if (request.Headers.Authorization != null && !request.Headers.Authorization.Scheme.Equals(Constants.OAuthAuthScheme))
            {
                OAuthRequestException.ThrowPermissionDenied("Invalid authorization scheme");
            }

            var oauthConsumerKey = string.Empty;
            var oauthSignature = string.Empty;
            var oauthParameters = new OAuthParameters();

            // Parse the Authorization parameter and extract the signature

            if (request.Headers.Authorization != null)
            {
                foreach (var pair in request.Headers.Authorization.Parameter.Split(','))
                {
                    var keyValue = pair.Split('=');
                    var key = keyValue[0].Trim();
                    var value = HttpUtility.UrlDecode(keyValue[1].Trim('"'));

                    // Ignore invalid key/value pairs
                    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) continue;

                    if (key.Equals("oauth_signature"))
                    {
                        oauthSignature = value;
                    }
                    else if (key.StartsWith("oauth_"))
                    {
                        if (key.Equals(Constants.ConsumerKeyParameter))
                        {
                            oauthConsumerKey = value;
                        }
                        oauthParameters.AdditionalParameters.Add(key, value);
                    }
                }
            }

            // Compare the body hash to make sure the content was not
            // tampered with

            using (var sha1 = new SHA1CryptoServiceProvider())
            using (var task = request.Content.ReadAsStreamAsync())
            {
                task.Wait(3000);
                if (!task.IsCompleted)
                {
                    OAuthRequestException.ThrowPermissionDenied("Timeout reading content");
                }

                task.Result.Position = 0;
                var hash = sha1.ComputeHash(task.Result);
                var hash64 = Convert.ToBase64String(hash);
                // Reset the position to 0 as a courtesy to others
                task.Result.Position = 0;

                if (!hash64.Equals(oauthParameters.AdditionalParameters["oauth_body_hash"]))
                {
                    OAuthRequestException.ThrowPermissionDenied("OAuth body hash does not match");
                }
            }

            // The key may be used by multiple providers, so scan them all
            // until a match is found
            var consumerSecrets = GetConsumerSecrets(oauthConsumerKey);
            if (consumerSecrets == null || !consumerSecrets.Any())
            {
                OAuthRequestException.ThrowConsumerKeyUnknown("Unknown consumer key");
            }

            var signatureProvider = new HmacSha1SigningProvider();
            var signatureBase = SignatureBase.Create("POST", request.RequestUri, oauthParameters);
            if (consumerSecrets != null && consumerSecrets
                .Select(consumerSecret => signatureProvider.ComputeSignature(signatureBase, consumerSecret, string.Empty))
                .Any(signature => signature.Equals(oauthSignature)))
            {
                return;
            }

            OAuthRequestException.ThrowSignatureInvalid("Unknown signature");
        }

        // POST api/outcomes

        [HttpPost]
        public imsx_POXEnvelopeType Post(imsx_POXEnvelopeType request)
        {
            WriteEnvelopeToDebug(request);

            imsx_POXEnvelopeType response = null;
            var requestHeader = request.imsx_POXHeader.Item as imsx_RequestHeaderInfoType;
            if (requestHeader == null)
            {
                response = CreateCustomResponse(string.Empty,
                    "Invalid request header",
                    imsx_CodeMajorType.failure);
            }
            else
            {
                // Authenticate and authorize the request
                var isAuthorized = false;
                try
                {
                    Authorize();
                    isAuthorized = true;
                }
                catch (OAuthRequestException ex)
                {
                    response = CreateCustomResponse(requestHeader.imsx_messageIdentifier,
                        ex.Advice,
                        imsx_CodeMajorType.failure);
                }

                if (isAuthorized)
                {
                    // All requests come in the same basic body element
                    var requestBody = request.imsx_POXBody;

                    // Delete Result
                    if (requestBody.Item is deleteResultRequest)
                    {
                        response = HandleDeleteResultRequest(requestHeader, requestBody);
                    }
                    else if (requestBody.Item is readResultRequest)
                    {
                        response = HandleReadResultRequest(requestHeader, requestBody);
                    }
                    else if (requestBody.Item is replaceResultRequest)
                    {
                        response = HandleReplaceResultRequest(requestHeader, requestBody);
                    }
                    else
                    {
                        response = CreateCustomResponse(requestHeader.imsx_messageIdentifier,
                            "Request is not supported",
                            imsx_CodeMajorType.unsupported);
                    }
                }
            }

            WriteEnvelopeToDebug(response);
            return response;
        }

        /// <summary>
        /// Create a simple, but complete response envelope.
        /// </summary>
        /// <param name="messageRefId">The request message ID.</param>
        /// <param name="description">The status description.</param>
        /// <param name="codeMajor">The custom status code.</param>
        /// <returns>A response envelope.</returns>
        private static imsx_POXEnvelopeType CreateCustomResponse(string messageRefId, string description, imsx_CodeMajorType codeMajor)
        {
            var response = CreateSuccessResponse(messageRefId, description);
            var header = (imsx_ResponseHeaderInfoType) response.imsx_POXHeader.Item;
            header.imsx_statusInfo.imsx_codeMajor = codeMajor;

            return response;
        }

        /// <summary>
        /// Create a simple, but complete response envelope. The status is set to success.
        /// </summary>
        /// <param name="messageRefId">The request message ID.</param>
        /// <param name="description">The status description.</param>
        /// <returns>A response envelope.</returns>
        private static imsx_POXEnvelopeType CreateSuccessResponse(string messageRefId, string description)
        {
            var response = new imsx_POXEnvelopeType();
            response.imsx_POXHeader = new imsx_POXHeaderType();
            response.imsx_POXHeader.Item = new imsx_ResponseHeaderInfoType();

            var item = response.imsx_POXHeader.Item as imsx_ResponseHeaderInfoType;
            item.imsx_version = imsx_GWSVersionValueType.V10;
            item.imsx_messageIdentifier = Guid.NewGuid().ToString();
            item.imsx_statusInfo = new imsx_StatusInfoType();

            var status = item.imsx_statusInfo;
            status.imsx_codeMajor = imsx_CodeMajorType.success;
            status.imsx_severity = imsx_SeverityType.status;
            status.imsx_description = description;
            status.imsx_messageRefIdentifier = messageRefId;

            response.imsx_POXBody = new imsx_POXBodyType();
            return response;
        }

        /// <summary>
        /// Convert the ResultRecordType into the Result type.
        /// </summary>
        /// <param name="resultRecord">The ResultRecordType which 
        /// specifies the assignment and score.</param>
        /// <returns>The corresponding Result</returns>
        private static Result GetResult(ResultRecordType resultRecord)
        {
            var result = new Result { SourcedGuid = resultRecord.sourcedGUID.sourcedId };
            if (resultRecord.result != null)
            {
                double value;
                if (double.TryParse(resultRecord.result.resultScore.textString, out value))
                {
                    if (value >= 0 && value <= 1)
                    {
                        result.DoubleValue = value;
                    }
                }
            }
            return result;
        }

        private imsx_POXEnvelopeType HandleDeleteResultRequest(imsx_RequestHeaderInfoType requestHeader, imsx_POXBodyType requestBody)
        {
            imsx_POXEnvelopeType response;
            var deleteRequest = requestBody.Item as deleteResultRequest ?? new deleteResultRequest();
            var deleteResponse = new deleteResultResponse();

            var result = GetResult(deleteRequest.resultRecord);
            if (DeleteResult(result))
            {
                response = CreateSuccessResponse(requestHeader.imsx_messageIdentifier,
                                                 string.Format("Score for {0} is deleted",
                                                               deleteRequest.resultRecord.sourcedGUID.sourcedId));
            }
            else
            {
                response = CreateCustomResponse(requestHeader.imsx_messageIdentifier,
                                                string.Format("Score for {0} not deleted",
                                                              deleteRequest.resultRecord.sourcedGUID.sourcedId),
                                                imsx_CodeMajorType.failure);
            }
            response.imsx_POXBody.Item = deleteResponse;
            return response;
        }

        private imsx_POXEnvelopeType HandleReadResultRequest(imsx_RequestHeaderInfoType requestHeader, imsx_POXBodyType requestBody)
        {
            imsx_POXEnvelopeType response;
            var readRequest = requestBody.Item as readResultRequest ?? new readResultRequest();
            var readResponse = new readResultResponse();

            var result = GetResult(readRequest.resultRecord);
            if (ReadResult(result))
            {
                if (result.DoubleValue == null)
                {
                    // The score could exist, but not found. If the grade has not yet been set via a replaceResult operation
                    // or an existing grade has been deleted via a deleteResult operation, the TC should return a valid
                    // response with a present but empty textString element.   The TC should not return 0.0 to indicate a
                    // non-existent grade and the TC should not return a failure status when a grade does not exist.
                    // It should simply return an "empty" grade.
                    response = CreateSuccessResponse(requestHeader.imsx_messageIdentifier,
                        string.Format("Score for {0} is null", readRequest.resultRecord.sourcedGUID.sourcedId));
                    readResponse.result = new ResultType();
                    readResponse.result.resultScore = new TextType();
                    var cultureInfo = new CultureInfo("en");
                    readResponse.result.resultScore.language = cultureInfo.Name;
                    readResponse.result.resultScore.textString = string.Empty;
                }
                else
                {
                    // The score exists
                    response = CreateSuccessResponse(requestHeader.imsx_messageIdentifier,
                        string.Format("Score for {0} is read", readRequest.resultRecord.sourcedGUID.sourcedId));
                    readResponse.result = new ResultType();
                    readResponse.result.resultScore = new TextType();
                    var cultureInfo = new CultureInfo("en");
                    readResponse.result.resultScore.language = cultureInfo.Name;
                    readResponse.result.resultScore.textString = result.DoubleValue.Value.ToString(cultureInfo);
                }
            }
            else
            {
                // The score could not exist (invalid assignment or user)
                response = CreateCustomResponse(requestHeader.imsx_messageIdentifier,
                    string.Format("Invalid score sourcedId {0}", readRequest.resultRecord.sourcedGUID.sourcedId),
                    imsx_CodeMajorType.failure);
            }
            response.imsx_POXBody.Item = readResponse;
            return response;
        }

        private imsx_POXEnvelopeType HandleReplaceResultRequest(imsx_RequestHeaderInfoType requestHeader, imsx_POXBodyType requestBody)
        {
            imsx_POXEnvelopeType response;
            var replaceRequest = requestBody.Item as replaceResultRequest ?? new replaceResultRequest();

            var result = GetResult(replaceRequest.resultRecord);
            if (!result.DoubleValue.HasValue)
            {
                response = CreateCustomResponse(requestHeader.imsx_messageIdentifier,
                    "Invalid result",
                    imsx_CodeMajorType.failure);
            }
            else if (ReplaceResult(result))
            {
                response = CreateSuccessResponse(requestHeader.imsx_messageIdentifier,
                    string.Format("Score for {0} is now {1}",
                        replaceRequest.resultRecord.sourcedGUID.sourcedId,
                        replaceRequest.resultRecord.result.resultScore.textString
                        ));
            }
            else
            {
                response = CreateCustomResponse(requestHeader.imsx_messageIdentifier,
                    string.Format("Score for {0} not replaced",
                        replaceRequest.resultRecord.sourcedGUID.sourcedId),
                    imsx_CodeMajorType.failure);
            }
            response.imsx_POXBody.Item = new replaceResultResponse();
            return response;
        }

// ReSharper disable UnusedParameter.Local
        private static void WriteEnvelopeToDebug(imsx_POXEnvelopeType envelope)
// ReSharper restore UnusedParameter.Local
        {
#if DEBUG
            using (var stream = new MemoryStream())
            {
                EnvelopeSerializer.Serialize(stream, envelope);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    Debug.WriteLine(reader.ReadToEnd());
                }
            }
            Debug.WriteLine(String.Empty);
#endif
        }
    }
}
