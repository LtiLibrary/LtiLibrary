using System;
using System.Linq;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Common;
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
    [AddBodyHashHeader]
    [Route("ims/courses/{contextId}/lineitems/{id?}", Name = "LineItemsApi")]
    [Consumes(LtiConstants.LisLineItemMediaType, LtiConstants.LisLineItemResultsMediaType, LtiConstants.LisLineItemContainerMediaType)]
    [Produces(LtiConstants.LisLineItemMediaType, LtiConstants.LisLineItemResultsMediaType, LtiConstants.LisLineItemContainerMediaType)]
    public abstract class LineItemsControllerBase : Controller
    {
        /// <summary>
        /// Initializes a new instance of the LineItemsControllerBase class.
        /// </summary>
        protected LineItemsControllerBase()
        {
            OnDeleteLineItem = dto => throw new NotImplementedException();
            OnGetLineItem = dto => throw new NotImplementedException();
            OnGetLineItemWithResults = dto => throw new NotImplementedException();
            OnGetLineItems = dto => throw new NotImplementedException();
            OnPostLineItem = dto => throw new NotImplementedException();
            OnPutLineItem = dto => throw new NotImplementedException();
            OnPutLineItemWithResults = dto => throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a particular LineItem instance in the Tool Consumer application.
        /// </summary>
        public Func<DeleteLineItemDto, Task> OnDeleteLineItem { get; set; }
        /// <summary>
        /// Get a representation of a particular LineItem instance from the Tool Consumer application.
        /// </summary>
        public Func<GetLineItemDto, Task> OnGetLineItem { get; set; }
        /// <summary>
        /// Get a representation of a particular LineItem instance with all its results in one call from the Tool Consumer application.
        /// </summary>
        public Func<GetLineItemDto, Task> OnGetLineItemWithResults { get; set; }
        /// <summary>
        /// Get a paginated list of LineItem resources from a LineItemContainer in the Tool Consumer application.
        /// </summary>
        public Func<GetLineItemsDto, Task> OnGetLineItems { get; set; }
        /// <summary>
        /// Create a new LineItem instance within the Tool Consumer Application.
        /// </summary>
        public Func<PostLineItemDto, Task> OnPostLineItem { get; set; }
        /// <summary>
        /// Update a particular LineItem instance in the Tool Consumer Application.
        /// </summary>
        public Func<PutLineItemDto, Task> OnPutLineItem { get; set; }
        /// <summary>
        /// Update a particular LineItem instance and the results it contains in the Tool Consumer Application.
        /// </summary>
        public Func<PutLineItemDto, Task> OnPutLineItemWithResults { get; set; }

        /// <summary>
        /// Delete a particular LineItem instance.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string contextId, string id)
        {
            try
            {
                var dto = new DeleteLineItemDto(contextId, id);

                await OnDeleteLineItem(dto);
                
                return StatusCode(dto.StatusCode);
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
                // Get a paginated list of LineItem resources from a LineItemContainer

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
                    var lineItemsDto = new GetLineItemsDto(contextId, limit, activityId, page);

                    await OnGetLineItems(lineItemsDto);

                    if (lineItemsDto.StatusCode == StatusCodes.Status200OK)
                    {
                        return new LineItemContainerPageResult(lineItemsDto.LineItemContainerPage)
                        {
                            StatusCode = lineItemsDto.StatusCode
                        };
                    }
                    return StatusCode(lineItemsDto.StatusCode);
                }

                // Get a representation of a particular LineItem instance

                var lineItemDto = new GetLineItemDto(contextId, id);
                var mediaType =
                    Request.Headers["Accept"].Contains(LtiConstants.LisLineItemResultsMediaType)
                        ? LtiConstants.LisLineItemResultsMediaType
                        : LtiConstants.LisLineItemMediaType;

                if (mediaType.Equals(LtiConstants.LisLineItemResultsMediaType))
                {
                    await OnGetLineItemWithResults(lineItemDto);
                }
                else
                {
                    await OnGetLineItem(lineItemDto);
                }

                if (lineItemDto.StatusCode == StatusCodes.Status200OK)
                {
                    if (mediaType == LtiConstants.LisLineItemResultsMediaType)
                    {
                        return new LineItemResultsResult(lineItemDto.LineItem);
                    }
                    return new LineItemResult(lineItemDto.LineItem);
                }
                return StatusCode(lineItemDto.StatusCode);
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
                var dto = new PostLineItemDto(contextId, lineItem);

                await OnPostLineItem(dto);

                if (dto.StatusCode == StatusCodes.Status201Created)
                {
                    return new LineItemResult(dto.LineItem, StatusCodes.Status201Created);
                }
                return StatusCode(dto.StatusCode);
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
                var dto = new PutLineItemDto(lineItem);

                var mediaType =
                    Request.Headers["Accept"].Contains(LtiConstants.LisLineItemResultsMediaType)
                        ? LtiConstants.LisLineItemResultsMediaType
                        : LtiConstants.LisLineItemMediaType;

                if (mediaType.Equals(LtiConstants.LisLineItemResultsMediaType))
                {
                    await OnPutLineItemWithResults(dto);
                }
                else
                {
                    await OnPutLineItem(dto);
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

