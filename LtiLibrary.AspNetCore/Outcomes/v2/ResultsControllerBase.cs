using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LtiLibrary.NetCore.Outcomes.v2;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lis.v2;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// An <see cref="Controller" /> that implements 
    /// "A REST API for LineItem Resources in multiple formats, Internal Draft 2.1"
    /// https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/outcomes/LISResult/service.html
    /// </summary>
    [AddBodyHashHeader]
    [Route("ims/courses/{contextId}/lineitems/{lineItemId}/results/{id?}", Name = "ResultsApi")]
    [Consumes(LtiConstants.LisResultMediaType, LtiConstants.LisResultContainerMediaType)]
    [Produces(LtiConstants.LisResultMediaType, LtiConstants.LisResultContainerMediaType)]
    public abstract class ResultsControllerBase : Controller
    {
        /// <summary>
        /// Initialize a new instance of the ResultsControllerBase class.
        /// </summary>
        protected ResultsControllerBase()
        {
            OnDeleteResult = dto => throw new NotImplementedException();
            OnGetResult = dto => throw new NotImplementedException();
            OnGetResults = dto => throw new NotImplementedException();
            OnPostResult = dto => throw new NotImplementedException();
            OnPutResult = dto => throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a particular LisResult instance in the Tool Consumer application.
        /// </summary>
        public Func<DeleteResultDto, Task> OnDeleteResult { get; set; }
        /// <summary>
        /// Get a representation of a particular LisResult instance from the Tool Consumer application.
        /// </summary>
        public Func<GetResultDto, Task> OnGetResult { get; set; }
        /// <summary>
        /// Get a paginated list of LisResult resources from an LisResultContainer in the Tool Consumer application.
        /// </summary>
        public Func<GetResultsDto, Task> OnGetResults { get; set; }
        /// <summary>
        /// Create a new LisResult instance within the Tool Consumer Application.
        /// </summary>
        public Func<PostResultDto, Task> OnPostResult { get; set; }
        /// <summary>
        /// Update a particular LisResult instance in the Tool Consumer Application.
        /// </summary>
        public Func<PutResultDto, Task> OnPutResult { get; set; }

        /// <summary>
        /// Delete a particular LisResult instance.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string contextId, string lineItemId, string id)
        {
            try
            {
                var dto = new DeleteResultDto(contextId, lineItemId, id);

                await OnDeleteResult(dto);

                return StatusCode(dto.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Get a paginated list of LisResult resources from a ResultContainer, or get a representation of a particular LisResult instance.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync(string contextId = null, string lineItemId = null, string id = null, int? limit = null, string firstPage = null, int? p = null)
        {
            try
            {
                // Get a paginated list of results from a ResultContainer
                if (string.IsNullOrEmpty(id))
                {
                    var page = 1;
                    if (p.HasValue)
                    {
                        if (firstPage != null && p.Value != 1)
                        {
                            return BadRequest("Request cannot specify both firstPage and a page number > 1");
                        }
                        page = p.Value;
                    }
                    var resultsDto = new GetResultsDto(contextId, lineItemId, limit, page);

                    await OnGetResults(resultsDto);

                    if (resultsDto.StatusCode == StatusCodes.Status200OK)
                    {
                        return new ResultContainerPageResult(resultsDto.ResultContainerPage)
                        {
                            StatusCode = resultsDto.StatusCode
                        };
                    }
                    return StatusCode(resultsDto.StatusCode);
                }

                // Get a representation of a particular LisResult instance

                var resultDto = new GetResultDto(contextId, lineItemId, id);

                await OnGetResult(resultDto);

                if (resultDto.StatusCode == StatusCodes.Status200OK)
                {
                    return new ResultResult(resultDto.Result)
                    {
                        StatusCode = resultDto.StatusCode
                    };
                }
                return StatusCode(resultDto.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Create a new LisResult instance.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostAsync(string contextId, string lineItemId, [ModelBinder(BinderType = typeof(LisResultModelBinder))] Result result)
        {
            try
            {
                var dto = new PostResultDto(contextId, lineItemId, result);

                await OnPostResult(dto);
                
                if (dto.StatusCode == StatusCodes.Status201Created)
                {
                    return new ResultResult(dto.Result)
                    {
                        StatusCode = dto.StatusCode
                    };
                }
                return StatusCode(dto.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Update a particular LisResult instance.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> PutAsync(string contextId, string lineItemId, string id, [ModelBinder(BinderType = typeof(LisResultModelBinder))] Result result)
        {
            try
            {
                var dto = new PutResultDto(contextId, lineItemId, id, result);

                await OnPutResult(dto);

                return StatusCode(dto.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}

