using System;
using System.Threading.Tasks;
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

        protected override Func<DeleteResultRequest, Task<DeleteResultResponse>> OnDeleteResultAsync => DeleteResultAsync;
        protected override Func<ReadResultRequest, Task<ReadResultResponse>> OnReadResultAsync => ReadResultAsync;
        protected override Func<ReplaceResultRequest, Task<ReplaceResultResponse>> OnReplaceResultAsync => ReplaceResultAsync;

        private async Task<DeleteResultResponse> DeleteResultAsync(DeleteResultRequest request)
        {
            var response = new DeleteResultResponse();

            var ltiRequest = await Request.ParseLtiRequestAsync();
            var signature = ltiRequest.GenerateSignature("secret");
            if (!ltiRequest.Signature.Equals(signature))
            {
                response.StatusCode = StatusCodes.Status401Unauthorized;
                return response;
            }

            _result = null;
            return response;
        }

        private async Task<ReadResultResponse> ReadResultAsync(ReadResultRequest request)
        {
            var response = new ReadResultResponse();

            var ltiRequest = await Request.ParseLtiRequestAsync();
            var signature = ltiRequest.GenerateSignature("secret");
            if (!ltiRequest.Signature.Equals(signature))
            {
                response.StatusCode = StatusCodes.Status401Unauthorized;
                return response;
            }

            response.Result = _result;
            return response;
        }

        private async Task<ReplaceResultResponse> ReplaceResultAsync(ReplaceResultRequest request)
        {
            var response= new ReplaceResultResponse();

            var ltiRequest = await Request.ParseLtiRequestAsync();
            var signature = ltiRequest.GenerateSignature("secret");
            if (!ltiRequest.Signature.Equals(signature))
            {
                response.StatusCode = StatusCodes.Status401Unauthorized;
                return response;
            }

            if (_result == null)
            {
                _result = new Result();
            }
            if (_result == null) return response;

            _result.Score = request.Result.Score;
            _result.SourcedId = request.Result.SourcedId;

            return response;
        }
    }
}
