using System.Collections.Generic;
using System.Runtime.Serialization;
using LtiLibrary.Core.Common;

namespace LtiLibrary.Core.Outcomes.v2
{
    /// <summary>
    /// A learning context, such as a Course Section.
    /// </summary>
    [DataContract]
    public class Context : JsonLdObject
    {
        public Context() : base(LtiConstants.ContextType)
        {
        }

        /// <summary>
        /// A unique string provided by the tool consumer to identify the context (as passed in the context_id launch parameter).
        /// </summary>
        [DataMember(Name = "context_id")]
        public string ContextId { get; set; }

        /// <summary>
        /// Zero or more LineItems that records results for some learning activity within this context.
        /// </summary>
        [DataMember(Name = "lineItem")]
        public ICollection<LineItem> LineItems { get; set; } 
    }
}
