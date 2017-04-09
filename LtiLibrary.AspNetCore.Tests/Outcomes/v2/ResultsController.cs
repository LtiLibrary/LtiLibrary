using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.AspNetCore.Outcomes.v2;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v2
{
    public class ResultsController : ResultsControllerBase
    {
        public ResultsController()
        {
            OnDeleteResult = async context =>
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

                var resultUri = new Uri(Url.Link("ResultsApi", new { context.ContextId, context.LineItemId, context.Id }));

                if (OutcomesDataFixture.Result == null || !OutcomesDataFixture.Result.Id.Equals(resultUri))
                {
                    context.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    OutcomesDataFixture.Result = null;
                    context.StatusCode = StatusCodes.Status200OK;
                }
            };

            OnGetResult = async context =>
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

                // https://www.imsglobal.org/specs/ltiomv2p0/specification-3
                // When a line item is created, a result for each user is deemed to be created with a status value of “Initialized”.  
                // Thus, there is no need to actually create a result with a POST request; the first connection to a result may be a PUT or a GET request.

                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { context.ContextId, id = context.LineItemId }));
                var resultUri = new Uri(Url.Link("ResultsApi", new { context.ContextId, context.LineItemId, context.Id }));

                if (OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = StatusCodes.Status404NotFound;
                }
                else if (OutcomesDataFixture.Result == null || !OutcomesDataFixture.Result.Id.Equals(resultUri))
                {
                    context.Result = new LisResult
                    {
                        ExternalContextId = LtiConstants.ResultContextId,
                        Id = resultUri,
                        ResultOf = lineItemUri,
                        ResultStatus = ResultStatus.Initialized
                    };
                    context.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    context.Result = OutcomesDataFixture.Result;
                    context.StatusCode = StatusCodes.Status200OK;
                }
            };

            OnGetResults = async context =>
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

                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { context.ContextId, id = context.LineItemId }));
                if (OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    context.ResultContainerPage = new ResultContainerPage
                    {
                        ExternalContextId = LtiConstants.ResultContainerContextId,
                        Id = new Uri(Url.Link("ResultsApi", new { context.ContextId, context.LineItemId })),
                        ResultContainer = new ResultContainer
                        {
                            MembershipSubject = new ResultMembershipSubject
                            {
                                Results = OutcomesDataFixture.Result == null ? new LisResult[] { } : new[] { OutcomesDataFixture.Result }
                            }
                        }
                    };
                    context.StatusCode = StatusCodes.Status200OK;
                }
            };

            OnPostResult = async context =>
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

                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { context.ContextId, id = context.LineItemId }));
                if (OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    OutcomesDataFixture.Result = context.Result;
                    OutcomesDataFixture.Result.Id = new Uri(Url.Link("ResultsApi", new { context.ContextId, context.LineItemId, id = OutcomesDataFixture.ResultId }));
                    context.Result = OutcomesDataFixture.Result;
                    context.StatusCode = StatusCodes.Status201Created;
                }
            };

            OnPutResult = async context =>
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

                // https://www.imsglobal.org/specs/ltiomv2p0/specification-3
                // When a line item is created, a result for each user is deemed to be created with a status value of “Initialized”.  
                // Thus, there is no need to actually create a result with a POST request; the first connection to a result may be a 
                // PUT or a GET request.

                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { context.ContextId, id = context.LineItemId }));
                if (OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    // If this is the first connection, the PUT is equivalent to a POST
                    OutcomesDataFixture.Result = context.Result;
                    if (context.Result.Id == null)
                    {
                        OutcomesDataFixture.Result.Id = new Uri(Url.Link("ResultsApi", new { context.ContextId, context.LineItemId, id = OutcomesDataFixture.ResultId }));
                    }
                    context.StatusCode = StatusCodes.Status200OK;
                }
            };
        }
    }
}
