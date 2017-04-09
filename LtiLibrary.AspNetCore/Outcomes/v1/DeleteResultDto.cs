using LtiLibrary.NetCore.Outcomes.v1;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// DTO to transfer data between the OutcomesControllerBase and a derived OutcomesController.
    /// </summary>
    public class DeleteResultDto
    {
        public DeleteResultDto(string lisResultSourcedId)
        {
            LisResultSourcedId = lisResultSourcedId;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// The LineItemId for this <see cref="LisResult"/>.
        /// </summary>
        public string LisResultSourcedId { get; set; }

        /// <summary>
        /// The HTTP StatusCode representing the result of the action (OK, NotFound, Unauthorized)
        /// </summary>
        public int StatusCode { get; set; }
    }
}
