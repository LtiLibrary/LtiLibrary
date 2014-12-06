using System.Collections.Generic;
using System.Runtime.Serialization;
using LtiLibrary.Common;
using LtiLibrary.Lti2;

namespace LtiLibrary.Profiles
{
    /// <summary>
    /// The ToolConsumerProfile advertises the Tool Consumer's capabilities and services offered.
    /// </summary>
    /// <remarks>
    /// The ToolConsumerProfile (and all of its members) conform to JSON-LD conventions. Briefly, JSON-LD adds 
    /// semantics to a JSON document by associating properties that may appear in a document with well-defined 
    /// data types through the use of a so-called "context".
    /// 
    /// The JSON-LD standard reserves a handful of property names and tokens that have special meaning. These 
    /// names and tokens, described below, begin with the '@' symbol.
    /// 
    /// @context - Used to reference (by URI or by value) a context which declares the simple names that appear 
    /// throughout a JSON document.
    /// 
    /// @id - Used to uniquely identify things that are being described in the JSON document. The value of an 
    /// @id property is either a fully-qualified URI, a CURIE, or a simple name that expands to a fully-qualified 
    /// URI by virtue of the rules defined in the JSON-LD Context.
    /// 
    /// @type - Used to set the data type of an object or property value.
    /// </remarks>
    [DataContract]
    public class ToolConsumerProfile
    {
        public ToolConsumerProfile(IEnumerable<string> capabilityOffered, string guid,
            string ltiVersion, ProductInstance productInstance) :

            this(new[]{LtiConstants.ToolConsumerProfileContext}, capabilityOffered, guid, 
                ltiVersion, productInstance)
        {
        }

        public ToolConsumerProfile(IEnumerable<string> context, IEnumerable<string> capabilityOffered, string guid, 
            string ltiVersion, ProductInstance productInstance)
        {
            // These are the required fields in a ToolConsumerProfile
            Context = context;
            Type = "ToolConsumerProfile";
            CapabilityOffered = capabilityOffered;
            Guid = guid;
            LtiVersion = ltiVersion;
            ProductInstance = productInstance;
        }

        /// <summary>
        /// For most implementations, the value will be the single URI for the standard context associated with 
        /// the application/vnd.ims.lti.v2.ToolConsumerProfile+json media type. In this case, the value will be
        /// <para>"http://purl.imsglobal.org/ctx/lti/v2/ToolConsumerProfile"</para>
        /// </summary>
        [DataMember(Name = "@context")]
        public IEnumerable<string> Context { get; set; }

        /// <summary>
        /// The URI that identifies this ToolConsumerProfile instance.
        /// </summary>
        [DataMember(Name = "@id")]
        public string Id { get; set; }

        /// <summary>
        /// A simple name identifying the object's type. The standard context [TCP-Context] defines the following 
        /// simple names that are applicable:
        /// <para>ToolConsumerProfile</para>
        /// <para>Implementations may use a custom JSON-LD context which defines simple names for additional types 
        /// that are subtypes of ToolConsumerProfile.</para>
        /// </summary>
        [DataMember(Name = "@type")]
        public string Type { get; private set; }

        /// <summary>
        /// A capability offered by the Tool Consumer to its integration partners.
        /// </summary>
        [DataMember(Name = "capability_offered")]
        public IEnumerable<string> CapabilityOffered { get; private set; }
        
        /// <summary>
        /// A globally unique identifier for the service provider. As a best practice, this value should match an 
        /// Internet domain name assigned by ICANN, but any globally unique identifier is acceptable. 
        /// </summary>
        [DataMember(Name = "guid")]
        public string Guid { get; private set; }

        /// <summary>
        /// The identifier for an LTI version that the version supports. A given product (Tool or Tool Consumer) 
        /// may support multiple versions, but only one version is selected for use in the integration contract. 
        /// Should match lti_version launch parameter.
        /// </summary>
        [DataMember(Name = "lti_version")]
        public string LtiVersion { get; private set; }

        /// <summary>
        /// An inverse attribute which references the ProductInstance within which this ToolConsumerProfile is defined.
        /// </summary>
        [DataMember(Name = "product_instance")]
        public ProductInstance ProductInstance { get; private set; }

        /// <summary>
        /// The descriptor for a service offered by the product (Tool or Tool Consumer).
        /// </summary>
        [DataMember(Name = "service_offered")]
        public IEnumerable<RestService> ServiceOffered { get; set; }
    }
}
