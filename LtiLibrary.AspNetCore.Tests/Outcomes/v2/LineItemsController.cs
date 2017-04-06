using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Outcomes.v2;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v2
{
    public class LineItemsController : LineItemsControllerBase
    {
        // Simple "database" of lineitems for demonstration purposes
        public const string ContextId = "course-1";
        public const string LineItemId = "lineitem-1";
        public const string ResultId = "result-1";
        public static LineItem LineItem;
        public static LisResult Result;

        public LineItemsController()
        {
            OnDeleteLineItem = context =>
            {
                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { ContextId, context.Id }));

                if (LineItem == null || !LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    LineItem = null;
                    Result = null;
                    context.StatusCode = StatusCodes.Status200OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItem = context =>
            {
                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { ContextId, context.Id }));

                if (LineItem == null || !LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    context.LineItem = LineItem;
                    context.StatusCode = StatusCodes.Status200OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItemWithResults = context =>
            {
                OnGetLineItem(context);
                if (context.LineItem != null && Result != null)
                {
                    context.LineItem.Result = Result == null ? new LisResult[] { } : new[] { Result };
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItems = context =>
            {
                context.LineItemContainerPage = new LineItemContainerPage
                {
                    ExternalContextId = LtiConstants.LineItemContainerContextId,
                    Id = new Uri(Url.Link("LineItemsApi", new { context.ContextId })),
                    LineItemContainer = new LineItemContainer
                    {
                        MembershipSubject = new LineItemMembershipSubject
                        {
                            ContextId = context.ContextId,
                            LineItems = new LineItem[] { }
                        }
                    }
                };

                if (LineItem != null &&
                    (string.IsNullOrEmpty(context.ActivityId) ||
                     context.ActivityId.Equals(LineItem.AssignedActivity.ActivityId)))
                {
                    context.LineItemContainerPage.LineItemContainer.MembershipSubject.LineItems = new[] { LineItem };
                }
                context.StatusCode = StatusCodes.Status200OK;
                return Task.FromResult<object>(null);
            };

            // Create a LineItem
            OnPostLineItem = context =>
            {
                if (LineItem != null)
                {
                    context.StatusCode = StatusCodes.Status400BadRequest;
                    return Task.FromResult<object>(null);
                }

                context.LineItem.Id = new Uri(Url.Link("LineItemsApi", new { context.ContextId, id = LineItemId }));
                context.LineItem.Results = new Uri(Url.Link("ResultsApi", new { context.ContextId, LineItemId }));
                LineItem = context.LineItem;
                context.StatusCode = StatusCodes.Status201Created;
                return Task.FromResult<object>(null);
            };

            // Update LineItem (but not results)
            OnPutLineItem = context =>
            {
                if (context.LineItem == null || LineItem == null || !LineItem.Id.Equals(context.LineItem.Id))
                {
                    context.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    context.LineItem.Result = LineItem.Result;
                    LineItem = context.LineItem;
                    context.StatusCode = StatusCodes.Status200OK;
                }
                return Task.FromResult<object>(null);
            };

            // Update LineItem and Result
            OnPutLineItemWithResults = context =>
            {
                if (context.LineItem == null || LineItem == null || !LineItem.Id.Equals(context.LineItem.Id))
                {
                    context.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    LineItem = context.LineItem;
                    if (context.LineItem.Result != null && context.LineItem.Result.Length > 0)
                    {
                        Result = context.LineItem.Result[0];
                    }
                    context.StatusCode = StatusCodes.Status200OK;
                }
                return Task.FromResult<object>(null);
            };
        }
    }
}
