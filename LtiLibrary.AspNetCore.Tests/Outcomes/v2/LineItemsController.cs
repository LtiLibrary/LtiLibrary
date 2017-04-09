using System;
using System.Net;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Extensions;
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
            OnDeleteLineItem = async context =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

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
            };

            OnGetLineItem = async context =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

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
            };

            OnGetLineItemWithResults = async context =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                await OnGetLineItem(context);

                if (context.LineItem != null && OutcomesDataFixture.Result != null)
                {
                    context.LineItem.Result = OutcomesDataFixture.Result == null ? new LisResult[] { } : new[] { OutcomesDataFixture.Result };
                }
            };

            OnGetLineItems = async context =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

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
            };

            // Create a LineItem
            OnPostLineItem = async context =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                if (OutcomesDataFixture.LineItem != null)
                {
                    context.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                context.LineItem.Id = new Uri(Url.Link("LineItemsApi", new { context.ContextId, id = OutcomesDataFixture.LineItemId }));
                context.LineItem.Results = new Uri(Url.Link("ResultsApi", new { context.ContextId, OutcomesDataFixture.LineItemId }));
                OutcomesDataFixture.LineItem = context.LineItem;
                context.StatusCode = StatusCodes.Status201Created;
            };

            // Update LineItem (but not results)
            OnPutLineItem = async context =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

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
            };

            // Update LineItem and Result
            OnPutLineItemWithResults = async context =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    context.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

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
            };
        }
    }
}
