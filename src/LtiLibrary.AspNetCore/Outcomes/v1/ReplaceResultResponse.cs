﻿using LtiLibrary.NetCore.Lti.v1;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// Represents a ReplaceResult response.
    /// </summary>
    public class ReplaceResultResponse
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public ReplaceResultResponse()
        {
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// The HTTP StatusCode representing the result of the action (OK, NotFound, Unauthorized)
        /// </summary>
        public int StatusCode { get; set; }
    }
}
