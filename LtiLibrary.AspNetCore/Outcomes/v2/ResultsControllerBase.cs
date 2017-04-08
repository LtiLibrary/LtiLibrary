using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LtiLibrary.NetCore.Outcomes.v2;
using LtiLibrary.NetCore.Common;

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
        protected ResultsControllerBase()
        {
            OnDeleteResult = context => throw new NotImplementedException();
            OnGetResult = context => throw new NotImplementedException();
            OnGetResults = context => throw new NotImplementedException();
            OnPostResult = context => throw new NotImplementedException();
            OnPutResult = context => throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a particular LisResult instance in the Tool Consumer application.
        /// </summary>
        public Func<DeleteResultContext, Task> OnDeleteResult { get; set; }
        /// <summary>
        /// Get a representation of a particular LisResult instance from the Tool Consumer application.
        /// </summary>
        public Func<GetResultContext, Task> OnGetResult { get; set; }
        /// <summary>
        /// Get a paginated list of LisResult resources from an LisResultContainer in the Tool Consumer application.
        /// </summary>
        public Func<GetResultsContext, Task> OnGetResults { get; set; }
        /// <summary>
        /// Create a new LisResult instance within the Tool Consumer Application.
        /// </summary>
        public Func<PostResultContext, Task> OnPostResult { get; set; }
        /// <summary>
        /// Update a particular LisResult instance in the Tool Consumer Application.
        /// </summary>
        public Func<PutResultContext, Task> OnPutResult { get; set; }

        /// <summary>
        /// Delete a particular LisResult instance.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string contextId, string lineItemId, string id)
        {
            try
            {
                var context = new DeleteResultContext(contextId, lineItemId, id);

                await OnDeleteResult(context);

                return StatusCode(context.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Get a paginated list of LisResult resources from a ResultContainer, or get a representation of a particular LisResult instance.
        /// <param name="id">The LineItem id.</param>
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync(string contextId = null, string lineItemId = null, string id = null, int? limit = null, string firstPage = null, int? p = null)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    // Get a paginated list of results from a ResultContainer

                    int page = 1;
                    if (p.HasValue)
                    {
                        if (firstPage != null && p.Value != 1)
                        {
                            return BadRequest("Request cannot specify both firstPage and a page number > 1");
                        }
                        page = p.Value;
                    }
                    var context = new GetResultsContext(contextId, lineItemId, limit, page);

                    await OnGetResults(context);

                    if (context.StatusCode == StatusCodes.Status200OK)
                    {
                        return new ResultContainerPageResult(context.ResultContainerPage)
                        {
                            StatusCode = context.StatusCode
                        };
                    }
                    else
                    {
                        return StatusCode(context.StatusCode);
                    }
                }
                else
                {
                    // Get a representation of a particular LisResult instance

                    var context = new GetResultContext(contextId, lineItemId, id);

                    await OnGetResult(context);

                    if (context.StatusCode == StatusCodes.Status200OK)
                    {
                        return new ResultResult(context.Result)
                        {
                            StatusCode = context.StatusCode
                        };
                    }
                    else
                    {
                        return StatusCode(context.StatusCode);
                    }
                }
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
        public async Task<IActionResult> PostAsync(string contextId, string lineItemId, [ModelBinder(BinderType = typeof(LisResultModelBinder))] LisResult result)
        {
            try
            {
                var context = new PostResultContext(contextId, lineItemId, result);

                await OnPostResult(context);

                if (context.StatusCode == StatusCodes.Status201Created)
                {
                    return new ResultResult(context.Result)
                    {
                        StatusCode = context.StatusCode
                    };
                }
                else
                {
                    return StatusCode(context.StatusCode);
                }
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
        public async Task<IActionResult> PutAsync(string contextId, string lineItemId, string id, [ModelBinder(BinderType = typeof(LisResultModelBinder))] LisResult result)
        {
            try
            {
                var context = new PutResultContext(contextId, lineItemId, id, result);

                await OnPutResult(context);

                return StatusCode(context.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}

