using System;
using System.Linq;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// An <see cref="Controller" /> that implements 
    /// "A REST API for LineItem Resources in multiple formats, Internal Draft 2.1"
    /// https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/outcomes/LineItem/service.html
    /// </summary>
    [Route("courses/{contextId}/lineitems/{id?}", Name = "LineItemsApi")]
    [Consumes(LtiConstants.LineItemMediaType, LtiConstants.LineItemResultsMediaType, LtiConstants.LineItemContainerMediaType)]
    [Produces(LtiConstants.LineItemMediaType, LtiConstants.LineItemResultsMediaType, LtiConstants.LineItemContainerMediaType)]
    public abstract class LineItemsControllerBase : Controller
    {
        protected LineItemsControllerBase()
        {
            OnDeleteLineItem = context => { throw new NotImplementedException(); };
            OnGetLineItem = context => { throw new NotImplementedException(); };
            OnGetLineItemWithResults = context => { throw new NotImplementedException(); };
            OnGetLineItems = context => { throw new NotImplementedException(); };
            OnPostLineItem = context => { throw new NotImplementedException(); };
            OnPutLineItem = context => { throw new NotImplementedException(); };
            OnPutLineItemWithResults = context => { throw new NotImplementedException(); };
        }

        /// <summary>
        /// Delete a particular LineItem instance in the Tool Consumer application.
        /// </summary>
        public Func<DeleteLineItemContext, Task> OnDeleteLineItem { get; set; }
        /// <summary>
        /// Get a representation of a particular LineItem instance from the Tool Consumer application.
        /// </summary>
        public Func<GetLineItemContext, Task> OnGetLineItem { get; set; }
        /// <summary>
        /// Get a representation of a particular LineItem instance with all its results in one call from the Tool Consumer application.
        /// </summary>
        public Func<GetLineItemContext, Task> OnGetLineItemWithResults { get; set; }
        /// <summary>
        /// Get a paginated list of LineItem resources from a LineItemContainer in the Tool Consumer application.
        /// </summary>
        public Func<GetLineItemsContext, Task> OnGetLineItems { get; set; }
        /// <summary>
        /// Create a new LineItem instance within the Tool Consumer Application.
        /// </summary>
        public Func<PostLineItemContext, Task> OnPostLineItem { get; set; }
        /// <summary>
        /// Update a particular LineItem instance in the Tool Consumer Application.
        /// </summary>
        public Func<PutLineItemContext, Task> OnPutLineItem { get; set; }
        /// <summary>
        /// Update a particular LineItem instance and the results it contains in the Tool Consumer Application.
        /// </summary>
        public Func<PutLineItemContext, Task> OnPutLineItemWithResults { get; set; }

        /// <summary>
        /// Delete a particular LineItem instance.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string contextId, string id)
        {
            try
            {
                var context = new DeleteLineItemContext(contextId, id);
                
                await OnDeleteLineItem(context);

                return StatusCode(context.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Get a paginated list of LineItem resources from a LineItemContainer, or get a representation of a particular LineItem instance.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync(string contextId = null, string id = null, int? limit = null, string activityId = null, string firstPage = null, int? p = null)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    // Get a paginated list of LineItem resources from a LineItemContainer

                    int page = 1;
                    if (p.HasValue)
                    {
                        if (firstPage != null && p.Value != 1)
                        {
                            return BadRequest("Request cannot specify both firstPage and a page number > 1");
                        }
                        page = p.Value;
                    }
                    var context = new GetLineItemsContext(contextId, limit, activityId, page);

                    await OnGetLineItems(context);

                    if (context.StatusCode == StatusCodes.Status200OK)
                    {
                        return new LineItemContainerPageResult(context.LineItemContainerPage)
                        {
                            StatusCode = context.StatusCode
                        };
                    }
                    return StatusCode(context.StatusCode);
                }
                else
                {
                    // Get a representation of a particular LineItem instance

                    var context = new GetLineItemContext(contextId, id);
                    var mediaType =
                        Request.Headers["Accept"].Contains(LtiConstants.LineItemResultsMediaType)
                            ? LtiConstants.LineItemResultsMediaType
                            : LtiConstants.LineItemMediaType;

                    if (mediaType.Equals(LtiConstants.LineItemResultsMediaType))
                    {
                        await OnGetLineItemWithResults(context);
                    }
                    else
                    {
                        await OnGetLineItem(context);
                    }

                    if (context.StatusCode == StatusCodes.Status200OK)
                    {
                        if (mediaType == LtiConstants.LineItemResultsMediaType)
                        {
                            return new LineItemResultsResult(context.LineItem);
                        }
                        else
                        {
                            return new LineItemResult(context.LineItem);
                        }
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
        /// Create a new LineItem instance.
        /// </summary>
        //[HttpPost("courses/{contextId}/lineitems/{id?}")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(string contextId, [ModelBinder(BinderType = typeof(LineItemModelBinder))] LineItem lineItem)
        {
            try
            {
                var context = new PostLineItemContext(contextId, lineItem);

                await OnPostLineItem(context);

                if (context.StatusCode == StatusCodes.Status201Created)
                {
                    return new LineItemResult(context.LineItem, StatusCodes.Status201Created);
                }
                return StatusCode(context.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Update a particular LineItem instance.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> PutAsync([ModelBinder(BinderType = typeof(LineItemModelBinder))] LineItem lineItem)
        {
            try
            {
                var context = new PutLineItemContext(lineItem);

                var mediaType =
                    Request.Headers["Accept"].Contains(LtiConstants.LineItemResultsMediaType)
                        ? LtiConstants.LineItemResultsMediaType
                        : LtiConstants.LineItemMediaType;

                if (mediaType.Equals(LtiConstants.LineItemResultsMediaType))
                {
                    await OnPutLineItemWithResults(context);
                }
                else
                {
                    await OnPutLineItem(context);
                }

                await OnPutLineItem(context);

                return StatusCode(context.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}

