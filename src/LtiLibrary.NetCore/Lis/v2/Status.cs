﻿using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v2
{
    /// <summary>
    /// Possible status values for LIS entities.
    /// http://purl.imsglobal.org/vocab/lis/v2/status
    /// </summary>
    [JsonConverter(typeof(StatusConverter))]
    public enum Status
    {
        /// <summary>
        /// The membership is active.
        /// </summary>
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/status#Active")]
        Active,

        /// <summary>
        /// The membership is deleted.
        /// </summary>
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/status#Deleted")]
        Deleted,

        /// <summary>
        /// The membership is inactive.
        /// </summary>
        [Uri("http://purl.imsglobal.org/vocab/lis/v2/status#Inactive")]
        Inactive
    }
}
