using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Outcomes.v2;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v2
{
    public class LineItemsController : LineItemsControllerBase
    {
        public LineItemsController()
        {
            OnDeleteLineItem = context =>
            {
                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { OutcomesDataFixture.ContextId, context.Id }));

                if (OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    OutcomesDataFixture.LineItem = null;
                    OutcomesDataFixture.Result = null;
                    context.StatusCode = StatusCodes.Status200OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItem = context =>
            {
                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { OutcomesDataFixture.ContextId, context.Id }));

                if (OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    context.LineItem = OutcomesDataFixture.LineItem;
                    context.StatusCode = StatusCodes.Status200OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItemWithResults = context =>
            {
                OnGetLineItem(context);
                if (context.LineItem != null && OutcomesDataFixture.Result != null)
                {
                    context.LineItem.Result = OutcomesDataFixture.Result == null ? new LisResult[] { } : new[] { OutcomesDataFixture.Result };
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

                if (OutcomesDataFixture.LineItem != null &&
                    (string.IsNullOrEmpty(context.ActivityId) ||
                     context.ActivityId.Equals(OutcomesDataFixture.LineItem.AssignedActivity.ActivityId)))
                {
                    context.LineItemContainerPage.LineItemContainer.MembershipSubject.LineItems = new[] { OutcomesDataFixture.LineItem };
                }
                context.StatusCode = StatusCodes.Status200OK;
                return Task.FromResult<object>(null);
            };

            // Create a LineItem
            OnPostLineItem = context =>
            {
                if (OutcomesDataFixture.LineItem != null)
                {
                    context.StatusCode = StatusCodes.Status400BadRequest;
                    return Task.FromResult<object>(null);
                }

                context.LineItem.Id = new Uri(Url.Link("LineItemsApi", new { context.ContextId, id = OutcomesDataFixture.LineItemId }));
                context.LineItem.Results = new Uri(Url.Link("ResultsApi", new { context.ContextId, OutcomesDataFixture.LineItemId }));
                OutcomesDataFixture.LineItem = context.LineItem;
                context.StatusCode = StatusCodes.Status201Created;
                return Task.FromResult<object>(null);
            };

            // Update LineItem (but not results)
            OnPutLineItem = context =>
            {
                if (context.LineItem == null || OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(context.LineItem.Id))
                {
                    context.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    context.LineItem.Result = OutcomesDataFixture.LineItem.Result;
                    OutcomesDataFixture.LineItem = context.LineItem;
                    context.StatusCode = StatusCodes.Status200OK;
                }
                return Task.FromResult<object>(null);
            };

            // Update LineItem and Result
            OnPutLineItemWithResults = context =>
            {
                if (context.LineItem == null || OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(context.LineItem.Id))
                {
                    context.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    OutcomesDataFixture.LineItem = context.LineItem;
                    if (context.LineItem.Result != null && context.LineItem.Result.Length > 0)
                    {
                        OutcomesDataFixture.Result = context.LineItem.Result[0];
                    }
                    context.StatusCode = StatusCodes.Status200OK;
                }
                return Task.FromResult<object>(null);
            };
        }
    }
}
