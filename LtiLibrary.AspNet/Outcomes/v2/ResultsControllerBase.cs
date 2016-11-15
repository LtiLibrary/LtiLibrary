using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    /// <summary>
    /// An <see cref="ApiController" /> that implements 
    /// "A REST API for LineItem Resources in multiple formats, Internal Draft 2.1"
    /// https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/outcomes/LISResult/service.html
    /// </summary>
    [ResultsControllerConfig]
    public abstract class ResultsControllerBase : ApiController
    {
        protected ResultsControllerBase()
        {
            OnDeleteResult = context => { throw new NotImplementedException(); };
            OnGetResult = context => { throw new NotImplementedException(); };
            OnGetResults = context => { throw new NotImplementedException(); };
            OnPostResult = context => { throw new NotImplementedException(); };
            OnPutResult = context => { throw new NotImplementedException(); };
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
        public async Task<HttpResponseMessage> DeleteAsync(string contextId, string lineItemId, string id)
        {
            try
            {
                var context = new DeleteResultContext(contextId, lineItemId, id);

                await OnDeleteResult(context);

                return Request.CreateResponse(context.StatusCode);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Get a paginated list of LisResult resources from a ResultContainer, or get a representation of a particular LisResult instance.
        /// <param name="id">The LineItem id.</param>
        /// </summary>
        [HttpGet]
        public async Task<HttpResponseMessage> GetAsync(string contextId = null, string lineItemId = null, string id = null, int? limit = null, string firstPage = null, int? p = null)
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
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                                new ArgumentException("Request cannot specify both firstPage and a page number > 1"));
                        }
                        page = p.Value;
                    }
                    var context = new GetResultsContext(contextId, lineItemId, limit, page);

                    await OnGetResults(context);

                    return context.StatusCode == HttpStatusCode.OK
                        ? Request.CreateResponse(context.StatusCode, context.ResultContainerPage, new ResultContainerPageFormatter())
                        : Request.CreateResponse(context.StatusCode);
                }
                else
                {
                    // Get a representation of a particular LisResult instance

                    var context = new GetResultContext(contextId, lineItemId, id);

                    await OnGetResult(context);

                    return context.StatusCode == HttpStatusCode.OK
                        ? Request.CreateResponse(context.StatusCode, context.Result, new ResultFormatter())
                        : Request.CreateResponse(context.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Create a new LisResult instance.
        /// </summary>
        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync(string contextId, string lineItemId, LisResult result)
        {
            try
            {
                var context = new PostResultContext(contextId, lineItemId, result);

                await OnPostResult(context);

                return context.StatusCode == HttpStatusCode.Created
                    ? Request.CreateResponse(context.StatusCode, context.Result, new ResultFormatter())
                    : Request.CreateResponse(context.StatusCode);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Update a particular LisResult instance.
        /// </summary>
        [HttpPut]
        public async Task<HttpResponseMessage> PutAsync(string contextId, string lineItemId, string id, LisResult result)
        {
            try
            {
                var context = new PutResultContext(contextId, lineItemId, id, result);

                await OnPutResult(context);

                return Request.CreateResponse(context.StatusCode);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}

