using LtiLibrary.Common;
using LtiLibrary.Extensions;
using LtiLibrary.Models;
using System;
using System.Globalization;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace LtiLibrary.Outcomes
{
    /// <summary>
    /// Implements the LTI Basic Outcomes API.
    /// </summary>
    public abstract class OutcomesApiControllerBase : ApiController
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            // The XSD code generator only creates one imsx_POXEnvelopeType which has the 
            // imsx_POXEnvelopeRequest root element. The IMS spec says the root element
            // should be imsx_POXEnvelopeResponse in the response.

            // Remove the default XmlFormatter that does not know how to override the root element
            var xmlFormatter = controllerContext.Configuration.Formatters.XmlFormatter;
            controllerContext.Configuration.Formatters.Remove(xmlFormatter);

            // Replace the default XmlFormatter with one that overrides the response root element
            var imsxXmlFormatter = new ImsxXmlMediaTypeFormatter();
            controllerContext.Configuration.Formatters.Add(imsxXmlFormatter);
        }

        /// <summary>
        /// Return true if the request is authorized.
        /// </summary>
        /// <param name="ltiRequest">The LtiOucomesRequest to authorize.</param>
        /// <returns>True if the request is authorized by the Tool Consumer.</returns>
        protected abstract bool IsAuthorized(LtiOutcomesRequest ltiRequest);

        /// <summary>
        /// Delete the result (grade, score, outcome) from the consumer.
        /// </summary>
        /// <param name="lisResultSourcedId">The sourcedId of the LisResult to delete.</param>
        /// <returns>True if the result was deleted.</returns>
        protected abstract bool DeleteResult(string lisResultSourcedId);

        /// <summary>
        /// Read the result (grade, score, outcome) from the consumer.
        /// </summary>
        /// <param name="lisResultSourcedId">The sourcedId of the LisResult to read.</param>
        /// <returns>The LisResult read. IsValid is true if the result is valid.</returns>
        protected abstract LisResult ReadResult(string lisResultSourcedId);

        /// <summary>
        /// Save or update the result (grade, score, outcome) in the consumer.
        /// </summary>
        /// <param name="result">The result to save or update.</param>
        /// <returns>True if the result was saved or updated.</returns>
        protected abstract bool ReplaceResult(LisResult result);

        /// <summary>
        /// Authenticate and authorize the request.
        /// </summary>
        private void Authorize()
        {
            var request = ControllerContext.Request.ParseIntoLtiOutcomesRequest();
            if (!IsAuthorized(request))
            {
                throw new LtiException("Not authorized");
            }
        }

        // POST api/outcomes

        [HttpPost]
        public imsx_POXEnvelopeType Post(imsx_POXEnvelopeType request)
        {
            imsx_POXEnvelopeType response = null;
            if (request == null)
            {
                response = CreateCustomResponse(string.Empty,
                    "Invalid request",
                    imsx_CodeMajorType.failure);
            }
            else
            {
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
                    catch (LtiException ex)
                    {
                        response = CreateCustomResponse(requestHeader.imsx_messageIdentifier,
                            ex.Message,
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
            }
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
        private static LisResult GetResult(ResultRecordType resultRecord)
        {
            var result = new LisResult { SourcedId = resultRecord.sourcedGUID.sourcedId };
            if (resultRecord.result != null)
            {
                // The LTI 1.1 specification states in 6.1.1. that the score in replaceResult should
                // always be formatted using “en” formatting
                // (http://www.imsglobal.org/LTI/v1p1p1/ltiIMGv1p1p1.html#_Toc330273034).
                const NumberStyles style = NumberStyles.Number | NumberStyles.AllowDecimalPoint;
                var culture = CultureInfo.CreateSpecificCulture(LtiConstants.ScoreLanguage);
                double value;
                if (double.TryParse(resultRecord.result.resultScore.textString, style, culture, out value))
                {
                    if (value >= 0 && value <= 1)
                    {
                        result.Score = value;
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
            if (DeleteResult(result.SourcedId))
            {
                response = CreateSuccessResponse(requestHeader.imsx_messageIdentifier,
                                                 string.Format("Score for {0} is deleted", result.SourcedId));
            }
            else
            {
                response = CreateCustomResponse(requestHeader.imsx_messageIdentifier,
                                                string.Format("Score for {0} not deleted", result.SourcedId),
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

            var result = ReadResult(readRequest.resultRecord.sourcedGUID.sourcedId);
            if (result.IsValid)
            {
                if (!result.Score.HasValue)
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
                    readResponse.result.resultScore.textString = result.Score.Value.ToString(cultureInfo);
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
            if (!result.Score.HasValue)
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
    }
}
