using LtiLibrary.NetCore.Outcomes.v1;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// DTO to transfer data between the OutcomesControllerBase and a derived OutcomesController.
    /// </summary>
    public class ReadResultDto
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public ReadResultDto(string lisResultSourcedId)
        {
            LisResultSourcedId = lisResultSourcedId;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// The <see cref="Result"/>.
        /// </summary>
        public Result Result { get; set; }

        /// <summary>
        /// The LineItemId for this <see cref="Result"/>.
        /// </summary>
        public string LisResultSourcedId { get; set; }

        /// <summary>
        /// The HTTP StatusCode representing the result of the action (OK, NotFound, Unauthorized)
        /// </summary>
        public int StatusCode { get; set; }
    }
}
