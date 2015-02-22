using System;
using System.Net;
using System.Threading.Tasks;
using LtiLibrary.AspNet.Outcomes.v2;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Tests
{
    public class LineItemsController : LineItemsControllerBase
    {
        // Simple "database" of scores for demonstration purposes
        public const string ContextId = "ltilibrary-context-1";
        public const string LineItemId = "ltilibrary-lineitem-1";
        private LineItem _lineItem;

        public LineItemsController()
        {
            OnDeleteLineItem = context =>
            {
                if (string.IsNullOrEmpty(context.Id) || _lineItem == null || !_lineItem.Id.Equals(context.Id))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    _lineItem = null;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItem = context =>
            {
                if (string.IsNullOrEmpty(context.Id) || _lineItem == null || !_lineItem.Id.Equals(context.Id))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    context.LineItem = _lineItem;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItems = context =>
            {
                if (_lineItem == null ||
                    (!string.IsNullOrEmpty(context.ActivityId) &&
                     !context.ActivityId.Equals(_lineItem.AssignedActivity.ActivityId)))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    var id = new UriBuilder(Request.RequestUri) { Query = "firstPage" };
                    context.LineItemContainerPage = new LineItemContainerPage
                    {
                        Id = id.ToString(),
                        LineItemContainer = new LineItemContainer
                        {
                            MembershipSubject = new Context
                            {
                                ContextId = _lineItem.LineItemOf.ContextId,
                                LineItems = new[] { _lineItem }
                            }
                        }
                    };
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnPostLineItem = context =>
            {
                if (_lineItem != null)
                {
                    context.StatusCode = HttpStatusCode.BadRequest;
                    return Task.FromResult<object>(null);
                }

                context.LineItem.Id = LineItemId;
                context.LineItem.Results = new Uri(Request.RequestUri.AbsoluteUri + LineItemId + "/results");
                _lineItem = context.LineItem;
                context.StatusCode = HttpStatusCode.Created;
                return Task.FromResult<object>(null);
            };

            OnPutLineItem = context =>
            {
                if (context.LineItem == null || _lineItem == null || !_lineItem.Id.Equals(context.LineItem.Id))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    _lineItem = context.LineItem;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };
        }
    }
}
