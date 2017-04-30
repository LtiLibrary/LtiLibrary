using System;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.AspNetCore.Outcomes.v2;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lis.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v2
{
    public class LineItemsController : LineItemsControllerBase
    {
        public LineItemsController()
        {
            OnDeleteLineItem = async dto =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { OutcomesDataFixture.ContextId, dto.Id }));

                if (OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(lineItemUri))
                {
                    dto.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    OutcomesDataFixture.LineItem = null;
                    OutcomesDataFixture.Result = null;
                    dto.StatusCode = StatusCodes.Status200OK;
                }
            };

            OnGetLineItem = async dto =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { OutcomesDataFixture.ContextId, dto.Id }));

                if (OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(lineItemUri))
                {
                    dto.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    dto.LineItem = OutcomesDataFixture.LineItem;
                    dto.StatusCode = StatusCodes.Status200OK;
                }
            };

            OnGetLineItemWithResults = async dto =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                await OnGetLineItem(dto);

                if (dto.LineItem != null && OutcomesDataFixture.Result != null)
                {
                    dto.LineItem.Result = OutcomesDataFixture.Result == null ? new Result[] { } : new[] { OutcomesDataFixture.Result };
                }
            };

            OnGetLineItems = async dto =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                dto.LineItemContainerPage = new LineItemContainerPage
                {
                    ExternalContextId = LtiConstants.LineItemContainerContextId,
                    Id = new Uri(Url.Link("LineItemsApi", new { dto.ContextId })),
                    LineItemContainer = new LineItemContainer
                    {
                        MembershipSubject = new LineItemMembershipSubject
                        {
                            ContextId = dto.ContextId,
                            LineItems = new LineItem[] { }
                        }
                    }
                };

                if (OutcomesDataFixture.LineItem != null &&
                    (string.IsNullOrEmpty(dto.ActivityId) ||
                     dto.ActivityId.Equals(OutcomesDataFixture.LineItem.AssignedActivity.ActivityId)))
                {
                    dto.LineItemContainerPage.LineItemContainer.MembershipSubject.LineItems = new[] { OutcomesDataFixture.LineItem };
                }
                dto.StatusCode = StatusCodes.Status200OK;
            };

            // Create a LineItem
            OnPostLineItem = async dto =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                if (OutcomesDataFixture.LineItem != null)
                {
                    dto.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                dto.LineItem.Id = new Uri(Url.Link("LineItemsApi", new { dto.ContextId, id = OutcomesDataFixture.LineItemId }));
                dto.LineItem.Results = new Uri(Url.Link("ResultsApi", new { dto.ContextId, OutcomesDataFixture.LineItemId }));
                OutcomesDataFixture.LineItem = dto.LineItem;
                dto.StatusCode = StatusCodes.Status201Created;
            };

            // Update LineItem (but not results)
            OnPutLineItem = async dto =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                if (dto.LineItem == null || OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(dto.LineItem.Id))
                {
                    dto.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    dto.LineItem.Result = OutcomesDataFixture.LineItem.Result;
                    OutcomesDataFixture.LineItem = dto.LineItem;
                    dto.StatusCode = StatusCodes.Status200OK;
                }
            };

            // Update LineItem and Result
            OnPutLineItemWithResults = async dto =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                if (dto.LineItem == null || OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(dto.LineItem.Id))
                {
                    dto.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    OutcomesDataFixture.LineItem = dto.LineItem;
                    if (dto.LineItem.Result != null && dto.LineItem.Result.Length > 0)
                    {
                        OutcomesDataFixture.Result = dto.LineItem.Result[0];
                    }
                    dto.StatusCode = StatusCodes.Status200OK;
                }
            };
        }
    }
}
