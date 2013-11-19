using System;
using LtiLibrary.Common;

namespace LtiLibrary.Models
{
    public class LtiInboundRequest : LtiRequest
    {
        public int LtiInboundRequestId { get; set; }
        public int ConsumerId { get; set; }
        public string Nonce { get; set; }
        public int OutcomeId { get; set; }
        public Int64 Timestamp { get; set; }
    }
}
