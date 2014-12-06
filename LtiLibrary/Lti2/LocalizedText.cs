using System.Runtime.Serialization;

namespace LtiLibrary.Lti2
{
    /// <summary>
    /// This container defines a block of text. The container includes a default value for the text, plus a 
    /// key that can be used to lookup a locale-specific value from a resource bundle.
    /// </summary>
    [DataContract]
    public class LocalizedText
    {
        /// <summary>
        /// The key used to lookup the locale-specific value from a resource bundle.
        /// </summary>
        [DataMember(Name = "key")]
        public string Key { get; set; }

        /// <summary>
        /// The default value for the text. This value is used if (1) the key attribute is undefined, 
        /// (2) the localization capability is not enabled, or (3) a value for the specified key is not found 
        /// in the locale-specific resource bundle.
        /// </summary>
        [DataMember(Name = "default_value")]
        public string Value { get; set; }
    }
}
