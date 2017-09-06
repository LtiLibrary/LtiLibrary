﻿using LtiLibrary.NetCore.Lti.v1;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// Represents a ReplaceResult.
    /// </summary>
    public class ReplaceResultRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public ReplaceResultRequest(Result result)
        {
            Result = result;
        }

        /// <summary>
        /// The <see cref="Result"/>.
        /// </summary>
        public Result Result { get; set; }
    }
}
