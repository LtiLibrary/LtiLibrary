using System;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.AspNetCore.Outcomes.v2;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lis.v2;
using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v2
{
    public class ResultsController : ResultsControllerBase
    {
        public ResultsController()
        {
            OnDeleteResult = async dto =>
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

                var resultUri = new Uri(Url.Link("ResultsApi", new { dto.ContextId, dto.LineItemId, dto.Id }));

                if (OutcomesDataFixture.Result == null || !OutcomesDataFixture.Result.Id.Equals(resultUri))
                {
                    dto.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    OutcomesDataFixture.Result = null;
                    dto.StatusCode = StatusCodes.Status200OK;
                }
            };

            OnGetResult = async dto =>
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

                // https://www.imsglobal.org/specs/ltiomv2p0/specification-3
                // When a line item is created, a result for each user is deemed to be created with a status value of “Initialized”.  
                // Thus, there is no need to actually create a result with a POST request; the first connection to a result may be a PUT or a GET request.

                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { dto.ContextId, id = dto.LineItemId }));
                var resultUri = new Uri(Url.Link("ResultsApi", new { dto.ContextId, dto.LineItemId, dto.Id }));

                if (OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(lineItemUri))
                {
                    dto.StatusCode = StatusCodes.Status404NotFound;
                }
                else if (OutcomesDataFixture.Result == null || !OutcomesDataFixture.Result.Id.Equals(resultUri))
                {
                    dto.Result = new Result
                    {
                        ExternalContextId = LtiConstants.ResultContextId,
                        Id = resultUri,
                        ResultOf = lineItemUri,
                        ResultStatus = ResultStatus.Initialized
                    };
                    dto.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    dto.Result = OutcomesDataFixture.Result;
                    dto.StatusCode = StatusCodes.Status200OK;
                }
            };

            OnGetResults = async dto =>
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

                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { dto.ContextId, id = dto.LineItemId }));
                if (OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(lineItemUri))
                {
                    dto.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    dto.ResultContainerPage = new ResultContainerPage
                    {
                        ExternalContextId = LtiConstants.ResultContainerContextId,
                        Id = new Uri(Url.Link("ResultsApi", new { dto.ContextId, dto.LineItemId })),
                        ResultContainer = new ResultContainer
                        {
                            MembershipSubject = new ResultMembershipSubject
                            {
                                Results = OutcomesDataFixture.Result == null ? new Result[] { } : new[] { OutcomesDataFixture.Result }
                            }
                        }
                    };
                    dto.StatusCode = StatusCodes.Status200OK;
                }
            };

            OnPostResult = async dto =>
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

                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { dto.ContextId, id = dto.LineItemId }));
                if (OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(lineItemUri))
                {
                    dto.StatusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    OutcomesDataFixture.Result = dto.Result;
                    OutcomesDataFixture.Result.Id = new Uri(Url.Link("ResultsApi", new { dto.ContextId, dto.LineItemId, id = OutcomesDataFixture.ResultId }));
                    dto.Result = OutcomesDataFixture.Result;
                    dto.StatusCode = StatusCodes.Status201Created;
                }
            };

            OnPutResult = async dto =>
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

                // https://www.imsglobal.org/specs/ltiomv2p0/specification-3
                // When a line item is created, a result for each user is deemed to be created with a status value of “Initialized”.  
                // Thus, there is no need to actually create a result with a POST request; the first connection to a result may be a 
                // PUT or a GET request.

                var lineItemUri = new Uri(Url.Link("LineItemsApi", new { dto.ContextId, id = dto.LineItemId }));
                if (OutcomesDataFixture.LineItem == null || !OutcomesDataFixture.LineItem.Id.Equals(lineItemUri))
                {
                    dto.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    // If this is the first connection, the PUT is equivalent to a POST
                    OutcomesDataFixture.Result = dto.Result;
                    if (dto.Result.Id == null)
                    {
                        OutcomesDataFixture.Result.Id = new Uri(Url.Link("ResultsApi", new { dto.ContextId, dto.LineItemId, id = OutcomesDataFixture.ResultId }));
                    }
                    dto.StatusCode = StatusCodes.Status200OK;
                }
            };
        }
    }
}
