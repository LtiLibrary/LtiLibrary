using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.AspNetCore.Outcomes.v1;
using LtiLibrary.NetCore.Lti.v1;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v1
{
    /// <summary>
    /// The OutcomesoApiController is hosted by the Tool Consumer and provides
    /// the functionality of the Outcomes Service API described in IMS LTI 1.1.
    /// </summary>
    public class OutcomesController : OutcomesControllerBase
    {
        // Simple "database" of scores for demonstration purposes
        private static Result _result;

        public OutcomesController()
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

                _result = null;
            };

            OnReadResult = async dto =>
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

                dto.Result = _result;
            };

            OnReplaceResult = async dto =>
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

                if (_result == null)
                {
                    _result = new Result();
                }
                if (_result != null)
                {
                    _result.Score = dto.Result.Score;
                    _result.SourcedId = dto.Result.SourcedId;
                }
            };
        }
    }
}
