using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    /// <summary>
    /// Based on A REST API for LineItem Resources in multiple formats, Internal Draft 2.1
    /// https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/outcomes/LineItem/service.html
    /// </summary>
    [LineItemsControllerConfig]
    public abstract class LineItemsControllerBase : ApiController
    {
        protected LineItemsControllerBase()
        {
            OnDeleteLineItem = context => { throw new NotImplementedException(); };
            OnGetLineItem = context => { throw new NotImplementedException(); };
            OnGetLineItems = context => { throw new NotImplementedException(); };
            OnPostLineItem = context => { throw new NotImplementedException(); };
            OnPutLineItem = context => { throw new NotImplementedException(); };
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
        /// Delete a particular LineItem instance.
        /// </summary>
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(string id)
        {
            try
            {
                var context = new DeleteLineItemContext(id);
                
                await OnDeleteLineItem(context);

                return Request.CreateResponse(context.StatusCode);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Get a paginated list of LineItem resources from a LineItemContainer, or get a representation of a particular LineItem instance.
        /// </summary>
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string id = null, int? limit = null, string activityId = null, string firstPage = null, int? p = null)
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
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                                new ArgumentException("Request cannot specify both firstPage and a page number > 1"));
                        }
                        page = p.Value;
                    }
                    var context = new GetLineItemsContext(limit, activityId, page);

                    await OnGetLineItems(context);

                    return context.StatusCode == HttpStatusCode.OK
                        ? Request.CreateResponse(context.StatusCode, context.LineItemContainerPage, new LineItemFormatter())
                        : Request.CreateResponse(context.StatusCode);
                }
                else
                {
                    // Get a representation of a particular LineItem instance

                    var context = new GetLineItemContext(id);

                    await OnGetLineItem(context);

                    return context.StatusCode == HttpStatusCode.OK
                        ? Request.CreateResponse(context.StatusCode, context.LineItem, new LineItemFormatter())
                        : Request.CreateResponse(context.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Create a new LineItem instance.
        /// </summary>
        [HttpPost]
        public async Task<HttpResponseMessage> Post(LineItem lineItem)
        {
            try
            {
                var context = new PostLineItemContext(lineItem);

                await OnPostLineItem(context);

                return context.StatusCode == HttpStatusCode.Created
                    ? Request.CreateResponse(context.StatusCode, context.LineItem, new LineItemFormatter()) 
                    : Request.CreateResponse(context.StatusCode);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Update a particular LineItem instance.
        /// </summary>
        [HttpPut]
        public async Task<HttpResponseMessage> Put(LineItem lineItem)
        {
            try
            {
                var context = new PutLineItemContext(lineItem);

                await OnPutLineItem(context);

                return Request.CreateResponse(context.StatusCode);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}

