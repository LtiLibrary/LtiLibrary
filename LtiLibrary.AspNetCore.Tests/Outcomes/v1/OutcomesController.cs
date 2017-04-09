using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.AspNetCore.Outcomes.v1;
using LtiLibrary.NetCore.Outcomes.v1;
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
        private static LisResult _lisResult;

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

                _lisResult = null;
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

                dto.LisResult = _lisResult;
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

                if (_lisResult == null)
                {
                    _lisResult = new LisResult();
                }
                _lisResult.Score = dto.LisResult.Score;
                _lisResult.SourcedId = dto.LisResult.SourcedId;
            };
        }
    }
}
