using System;
using System.Globalization;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Common;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Outcomes.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// Implements the LTI Basic Outcomes service introduced in LTI 1.1.
    /// </summary>
    [AddBodyHashHeader]
    [Route("ims/outcomes", Name = "OutcomesApi")]
    [Consumes("application/xml")]
    [Produces("application/xml")]
    public abstract class OutcomesControllerBase : Controller
    {
        /// <summary>
        /// Initializes a new instance of the OutcomesControllerBase.
        /// </summary>
        protected OutcomesControllerBase()
        {
            OnDeleteResult = dto => throw new NotImplementedException();
            OnReadResult = dto => throw new NotImplementedException();
            OnReplaceResult = dto => throw new NotImplementedException();
        }

        /// <summary>
        /// Delete the result (grade, score, outcome) from the consumer.
        /// </summary>
        public Func<DeleteResultDto, Task> OnDeleteResult { get; set; }
        /// <summary>
        /// Read the result (grade, score, outcome) from the consumer.
        /// </summary>
        public Func<ReadResultDto, Task> OnReadResult { get; set; }
        /// <summary>
        /// Save or update the result (grade, score, outcome) in the consumer.
        /// </summary>
        public Func<ReplaceResultDto, Task> OnReplaceResult { get; set; }

        // POST api/outcomes
        [HttpPost]
        public async Task<IActionResult> Post([ModelBinder(BinderType = typeof(ImsxXmlMediaTypeModelBinder))] imsx_POXEnvelopeType request)
        {
            if (request == null)
            {
                return BadRequest("Empty request.");
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
                return await HandleDeleteResultRequest(requestHeader, requestBody);
            }
            if (requestBody.Item is readResultRequest)
            {
                return await HandleReadResultRequest(requestHeader, requestBody);
            }
            if (requestBody.Item is replaceResultRequest)
            {
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
        private static LisResult GetResult(ResultRecordType resultRecord)
        {
            var result = new LisResult { SourcedId = resultRecord.sourcedGUID.sourcedId };
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
                var dto = new DeleteResultDto(result.SourcedId);

                await OnDeleteResult(dto);

                if (dto.StatusCode == StatusCodes.Status200OK)
                {
                    return CreateSuccessResponse(deleteResponse, requestHeader.imsx_messageIdentifier,
                        $"Score for {result.SourcedId} is deleted");
                }
                return StatusCode(dto.StatusCode);
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
                var dto = new ReadResultDto(readRequest.resultRecord.sourcedGUID.sourcedId);

                await OnReadResult(dto);

                if (dto.StatusCode == StatusCodes.Status200OK)
                {
                    if (dto.LisResult != null)
                    {
                        if (dto.LisResult.Score.HasValue)
                        {
                            // The score exists
                            readResponse.result = new ResultType { resultScore = new TextType() };
                            var cultureInfo = new CultureInfo("en");
                            readResponse.result.resultScore.language = cultureInfo.Name;
                            readResponse.result.resultScore.textString = dto.LisResult.Score.Value.ToString(cultureInfo);
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
                return StatusCode(dto.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        private async Task<IActionResult> HandleReplaceResultRequest(imsx_RequestHeaderInfoType requestHeader, imsx_POXBodyType requestBody)
        {
            var replaceRequest = requestBody.Item as replaceResultRequest ?? new replaceResultRequest();

            var result = GetResult(replaceRequest.resultRecord);
            try
            {
                var dto = new ReplaceResultDto(result);

                await OnReplaceResult(dto);

                if (dto.StatusCode == StatusCodes.Status200OK)
                {
                    return CreateSuccessResponse(replaceRequest, requestHeader.imsx_messageIdentifier,
                        $"Score for {replaceRequest.resultRecord.sourcedGUID.sourcedId} is now {replaceRequest.resultRecord.result.resultScore.textString}");
                }
                return StatusCode(dto.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
