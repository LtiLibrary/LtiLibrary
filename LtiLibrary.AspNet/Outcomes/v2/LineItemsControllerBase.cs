using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
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

        public Func<DeleteLineItemContext, Task> OnDeleteLineItem { get; set; }
        public Func<GetLineItemContext, Task> OnGetLineItem { get; set; }
        public Func<GetLineItemsContext, Task> OnGetLineItems { get; set; }
        public Func<PostLineItemContext, Task> OnPostLineItem { get; set; }
        public Func<PutLineItemContext, Task> OnPutLineItem { get; set; }

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

        [HttpGet]
        public async Task<HttpResponseMessage> Get(string id = null, int? limit = null, string activityId = null, string firstPage = null, int? p = null)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
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
                        ? Request.CreateResponse(context.StatusCode, context.LineItemContainerPage)
                        : Request.CreateResponse(context.StatusCode);
                }
                else
                {
                    var context = new GetLineItemContext(id);

                    await OnGetLineItem(context);

                    return context.StatusCode == HttpStatusCode.OK
                        ? Request.CreateResponse(context.StatusCode, context.LineItem)
                        : Request.CreateResponse(context.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(LineItem lineItem)
        {
            try
            {
                var context = new PostLineItemContext(lineItem);

                await OnPostLineItem(context);

                return context.StatusCode == HttpStatusCode.Created 
                    ? Request.CreateResponse(context.StatusCode, context.LineItem) 
                    : Request.CreateResponse(context.StatusCode);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

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

