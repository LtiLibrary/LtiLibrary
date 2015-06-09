using Newtonsoft.Json;

namespace LtiLibrary.Core.Lti2
{
    /// <summary>
    /// This container stores the default display name for some object plus a key that may be used to lookup 
    /// a translation for the display name from a resource bundle for a particular locale. The string value 
    /// has a maximum length of 128 characters.
    /// </summary>
    public class LocalizedName
    {
        /// <summary>
        /// The key used to lookup the locale-specific value from a resource bundle.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; protected set; }

        /// <summary>
        /// The default value for the display name. This value is used if (1) the key attribute is undefined, 
        /// (2) the localization capability is not enabled, or (3) a value for the specified key is not found 
        /// in the locale-specific resource bundle.
        /// </summary>
        [JsonProperty("default_value")]
        public string Value { get; protected set; }
    }
}
