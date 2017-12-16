using System;
using System.Globalization;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Common;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// A <see cref="Controller" /> that implements the LTI Outcomes Management 1.0 service.
    /// https://www.imsglobal.org/specs/ltiomv1p0/specification
    /// </summary>
    /// <remarks>
    /// Unless it is overridden, the route for this controller will be "ims/[controller]" named "OutcomesApi".
    /// </remarks>
    [AddBodyHashHeader]
    [Route("ims/[controller]", Name = "OutcomesApi")]
    [Consumes("application/xml")]
    [Produces("application/xml")]
    public abstract class OutcomesControllerBase : Controller
    {
        /// <summary>
        /// Delete the result (grade, score, outcome) from the consumer.
        /// </summary>
        protected abstract Func<DeleteResultRequest, Task<DeleteResultResponse>> OnDeleteResultAsync { get; }
        /// <summary>
        /// Read the result (grade, score, outcome) from the consumer.
        /// </summary>
        protected abstract Func<ReadResultRequest, Task<ReadResultResponse>> OnReadResultAsync { get; }
        /// <summary>
        /// Save or update the result (grade, score, outcome) in the consumer.
        /// </summary>
        protected abstract Func<ReplaceResultRequest, Task<ReplaceResultResponse>> OnReplaceResultAsync { get; }

        /// <summary>
        /// Receive the Outcomes 1.0 Post request.
        /// </summary>
        [HttpPost]
        public virtual async Task<IActionResult> Post([ModelBinder(BinderType = typeof(ImsxXmlMediaTypeModelBinder))] imsx_POXEnvelopeType request)
        {
            if (request == null)
            {
                return BadRequest("Empty request.");
            }

            // Check for required Authorization header with parameters dictated by the OAuth Body Hash Protocol
            if (!Request.IsAuthenticatedWithLti())
            {
                return Unauthorized();
            }

            if (!(request.imsx_POXHeader.Item is imsx_RequestHeaderInfoType requestHeader))
            {
                return BadRequest("Invalid request header.");
            }

            // All requests come in the same basic body element
            var requestBody = request.imsx_POXBody;

            // Delete Result
            switch (requestBody.Item)
            {
                case deleteResultRequest _:
                    if (OnDeleteResultAsync == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    return await HandleDeleteResultRequest(requestHeader, requestBody)
                        .ConfigureAwait(false);
                case readResultRequest _:
                    if (OnReadResultAsync == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    return await HandleReadResultRequest(requestHeader, requestBody)
                        .ConfigureAwait(false);
                case replaceResultRequest _:
                    if (OnReplaceResultAsync == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    return await HandleReplaceResultRequest(requestHeader, requestBody)
                        .ConfigureAwait(false);
            }
            return BadRequest("Request type not supported.");
        }

        /// <summary>
        /// Create a simple, but complete response envelope. The status is set to failure.
        /// </summary>
        /// <param name="responseItem">The ismx response element.</param>
        /// <param name="messageRefId">The request message ID.</param>
        /// <param name="statusCode">The HTTP status code that most closely corresponds to the failure.</param>
        /// <param name="description">The status description.</param>
        /// <returns>A response envelope.</returns>
        private static ImsxXmlMediaTypeResult CreateFailureResponse(object responseItem, string messageRefId, int statusCode, string description)
        {
            var response = new imsx_POXEnvelopeType
            {
                imsx_POXHeader = new imsx_POXHeaderType { Item = new imsx_ResponseHeaderInfoType() }
            };

            var item = (imsx_ResponseHeaderInfoType)response.imsx_POXHeader.Item;
            item.imsx_version = imsx_GWSVersionValueType.V10;
            item.imsx_messageIdentifier = Guid.NewGuid().ToString();
            item.imsx_statusInfo = new imsx_StatusInfoType();

            var status = item.imsx_statusInfo;
            status.imsx_codeMajor = imsx_CodeMajorType.failure;
            status.imsx_severity = imsx_SeverityType.error;
            status.imsx_description = description;
            status.imsx_messageRefIdentifier = messageRefId;

            response.imsx_POXBody = new imsx_POXBodyType { Item = responseItem };
            return new ImsxXmlMediaTypeResult(response) {StatusCode = statusCode};
        }

        /// <summary>
        /// Create a simple, but complete response envelope. The status is set to success.
        /// </summary>
        /// <param name="responseItem">The ismx response element.</param>
        /// <param name="messageRefId">The request message ID.</param>
        /// <param name="description">The status description.</param>
        /// <returns>A response envelope.</returns>
        private static ImsxXmlMediaTypeResult CreateSuccessResponse(object responseItem, string messageRefId, string description)
        {
            var response = new imsx_POXEnvelopeType
            {
                imsx_POXHeader = new imsx_POXHeaderType {Item = new imsx_ResponseHeaderInfoType()}
            };

            var item = (imsx_ResponseHeaderInfoType) response.imsx_POXHeader.Item;
            item.imsx_version = imsx_GWSVersionValueType.V10;
            item.imsx_messageIdentifier = Guid.NewGuid().ToString();
            item.imsx_statusInfo = new imsx_StatusInfoType();

            var status = item.imsx_statusInfo;
            status.imsx_codeMajor = imsx_CodeMajorType.success;
            status.imsx_severity = imsx_SeverityType.status;
            status.imsx_description = description;
            status.imsx_messageRefIdentifier = messageRefId;

            response.imsx_POXBody = new imsx_POXBodyType {Item = responseItem};
            return new ImsxXmlMediaTypeResult(response);
        }

        /// <summary>
        /// Convert the ResultRecordType into the Result type.
        /// </summary>
        /// <param name="resultRecord">The ResultRecordType which 
        /// specifies the assignment and score.</param>
        /// <returns>The corresponding Result</returns>
        private static Result GetResult(ResultRecordType resultRecord)
        {
            var result = new Result { SourcedId = resultRecord.sourcedGUID.sourcedId };
            if (resultRecord.result != null)
            {
                // The LTI 1.1 specification states in 6.1.1. that the score in replaceResult should
                // always be formatted using “en” formatting
                // (http://www.imsglobal.org/LTI/v1p1p1/ltiIMGv1p1p1.html#_Toc330273034).
                const NumberStyles style = NumberStyles.Number | NumberStyles.AllowDecimalPoint;
                var culture = new CultureInfo(LtiConstants.ScoreLanguage);
                if (double.TryParse(resultRecord.result.resultScore.textString, style, culture, out var value))
                {
                    if (value >= 0 && value <= 1)
                    {
                        result.Score = value;
                    }
                }
            }
            return result;
        }

        private async Task<IActionResult> HandleDeleteResultRequest(imsx_RequestHeaderInfoType requestHeader, imsx_POXBodyType requestBody)
        {
            var deleteRequest = requestBody.Item as deleteResultRequest ?? new deleteResultRequest();
            var deleteResponse = new deleteResultResponse();

            var result = GetResult(deleteRequest.resultRecord);
            try
            {
                var request = new DeleteResultRequest(result.SourcedId);
                var response = await OnDeleteResultAsync(request);

                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return CreateFailureResponse(deleteResponse, requestHeader.imsx_messageIdentifier,
                        response.StatusCode, response.StatusDescription);
                }
                return CreateSuccessResponse(deleteResponse, requestHeader.imsx_messageIdentifier,
                    response.StatusDescription ?? $"Score for {result.SourcedId} is deleted");
            }
            catch (Exception ex)
            {
                return CreateFailureResponse(deleteResponse, requestHeader.imsx_messageIdentifier,
                    StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private async Task<IActionResult> HandleReadResultRequest(imsx_RequestHeaderInfoType requestHeader, imsx_POXBodyType requestBody)
        {
            var readRequest = requestBody.Item as readResultRequest ?? new readResultRequest();
            var readResponse = new readResultResponse();

            try
            {
                var request = new ReadResultRequest(readRequest.resultRecord.sourcedGUID.sourcedId);
                var response = await OnReadResultAsync(request);

                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return CreateFailureResponse(readResponse, requestHeader.imsx_messageIdentifier,
                        response.StatusCode, response.StatusDescription);
                }

                if (response.Result == null)
                {
                    return CreateFailureResponse(readResponse, requestHeader.imsx_messageIdentifier,
                        response.StatusCode, response.StatusDescription ?? "TC did not read result.");
                }

                if (response.Result.Score.HasValue)
                {
                    // The score exists
                    readResponse.result = new ResultType { resultScore = new TextType() };
                    var cultureInfo = new CultureInfo("en");
                    readResponse.result.resultScore.language = cultureInfo.Name;
                    readResponse.result.resultScore.textString = response.Result.Score.Value.ToString(cultureInfo);
                    return CreateSuccessResponse(readResponse, requestHeader.imsx_messageIdentifier,
                        response.StatusDescription ?? $"Score for {readRequest.resultRecord.sourcedGUID.sourcedId} is read");
                }
                else
                {
                    // The score could exist, but not found. If the grade has not yet been set via a replaceResult operation
                    // or an existing grade has been deleted via a deleteResult operation, the TC should return a valid
                    // response with a present but empty textString element. The TC should not return 0.0 to indicate a
                    // non-existent grade and the TC should not return a failure status when a grade does not exist.
                    // It should simply return an "empty" grade.
                    readResponse.result = new ResultType { resultScore = new TextType() };
                    var cultureInfo = new CultureInfo("en");
                    readResponse.result.resultScore.language = cultureInfo.Name;
                    readResponse.result.resultScore.textString = string.Empty;
                    return CreateSuccessResponse(readResponse, requestHeader.imsx_messageIdentifier,
                        response.StatusDescription ?? $"Score for {readRequest.resultRecord.sourcedGUID.sourcedId} is null");
                }
            }
            catch (Exception ex)
            {
                return CreateFailureResponse(readResponse, requestHeader.imsx_messageIdentifier,
                    StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private async Task<IActionResult> HandleReplaceResultRequest(imsx_RequestHeaderInfoType requestHeader, imsx_POXBodyType requestBody)
        {
            var replaceRequest = requestBody.Item as replaceResultRequest ?? new replaceResultRequest();
            var replaceResponse = new replaceResultResponse();

            var result = GetResult(replaceRequest.resultRecord);

            //The LTI 1.1 specification states in 6.1.1:
            //The TC must check the incoming grade for validity and must fail when a grade is outside the range 0.0-1.0 or if the grade is not a valid number.   
            //The TC must respond to these replaceResult operations with a imsx_codeMajor of "failure".
            if (!result.Score.HasValue || !(result.Score >= 0 && result.Score <= 1))
            {
                return CreateFailureResponse(replaceResponse, requestHeader.imsx_messageIdentifier, StatusCodes.Status400BadRequest,
                    "The result score must be a decimal value in the range 0.0 - 1.0");
            }

            try
            {
                var request = new ReplaceResultRequest(result);
                var response = await OnReplaceResultAsync(request);

                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return CreateFailureResponse(replaceResponse, requestHeader.imsx_messageIdentifier,
                       response.StatusCode, response.StatusDescription);
                }
                return CreateSuccessResponse(replaceResponse, requestHeader.imsx_messageIdentifier,
                    response.StatusDescription
                    ?? $"Score for {replaceRequest.resultRecord.sourcedGUID.sourcedId} is now {replaceRequest.resultRecord.result.resultScore.textString}");
            }
            catch (Exception ex)
            {
                return CreateFailureResponse(replaceResponse, requestHeader.imsx_messageIdentifier,
                    StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
