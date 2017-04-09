using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Outcomes.v1;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// DTO to transfer data between the OutcomesControllerBase and a derived OutcomesController.
    /// </summary>
    public class ReadResultDto
    {
        public ReadResultDto(string lisResultSourcedId)
        {
            LisResultSourcedId = lisResultSourcedId;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// The <see cref="LisResult"/>.
        /// </summary>
        public LisResult LisResult { get; set; }

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
