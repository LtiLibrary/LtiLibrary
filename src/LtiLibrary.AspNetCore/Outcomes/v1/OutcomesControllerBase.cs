﻿using System;
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

            var requestHeader = request.imsx_POXHeader.Item as imsx_RequestHeaderInfoType;
            if (requestHeader == null)
            {
                return BadRequest("Invalid request header.");
            }

            // All requests come in the same basic body element
            var requestBody = request.imsx_POXBody;

            // Delete Result
            if (requestBody.Item is deleteResultRequest)
            {
                if (OnDeleteResultAsync == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                return await HandleDeleteResultRequest(requestHeader, requestBody);
            }
            if (requestBody.Item is readResultRequest)
            {
                if (OnReadResultAsync == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                return await HandleReadResultRequest(requestHeader, requestBody);
            }
            if (requestBody.Item is replaceResultRequest)
            {
                if (OnReplaceResultAsync == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                return await HandleReplaceResultRequest(requestHeader, requestBody);
            }
            return BadRequest("Request type not supported.");
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

        private async Task<IActionResult> HandleDeleteResultRequest(imsx_RequestHeaderInfoType requestHeader, imsx_POXBodyType requestBody)
        {
            var deleteRequest = requestBody.Item as deleteResultRequest ?? new deleteResultRequest();
            var deleteResponse = new deleteResultResponse();

            var result = GetResult(deleteRequest.resultRecord);
            try
            {
                var request = new DeleteResultRequest(result.SourcedId);
                var response = await OnDeleteResultAsync(request);

                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return CreateSuccessResponse(deleteResponse, requestHeader.imsx_messageIdentifier,
                        $"Score for {result.SourcedId} is deleted");
                }
                return StatusCode(response.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
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

                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    if (response.Result != null)
                    {
                        if (response.Result.Score.HasValue)
                        {
                            // The score exists
                            readResponse.result = new ResultType { resultScore = new TextType() };
                            var cultureInfo = new CultureInfo("en");
                            readResponse.result.resultScore.language = cultureInfo.Name;
                            readResponse.result.resultScore.textString = response.Result.Score.Value.ToString(cultureInfo);
                            return CreateSuccessResponse(readResponse, requestHeader.imsx_messageIdentifier,
                                $"Score for {readRequest.resultRecord.sourcedGUID.sourcedId} is read");
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
                                $"Score for {readRequest.resultRecord.sourcedGUID.sourcedId} is null");
                        }
                    }
                }
                return StatusCode(response.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        private async Task<IActionResult> HandleReplaceResultRequest(imsx_RequestHeaderInfoType requestHeader, imsx_POXBodyType requestBody)
        {
            var replaceRequest = requestBody.Item as replaceResultRequest ?? new replaceResultRequest();
            var replaceResponse = new replaceResultResponse();

            var result = GetResult(replaceRequest.resultRecord);
            try
            {
                var request = new ReplaceResultRequest(result);
                var response = await OnReplaceResultAsync(request);

                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return CreateSuccessResponse(replaceResponse, requestHeader.imsx_messageIdentifier,
                        $"Score for {replaceRequest.resultRecord.sourcedGUID.sourcedId} is now {replaceRequest.resultRecord.result.resultScore.textString}");
                }
                return StatusCode(response.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
