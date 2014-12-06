using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable InconsistentNaming
namespace LtiLibrary.Lti1
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DocumentTarget
    {
        embed,
        frame,
        iframe,
        overlay,
        popup,
        window
    }
}
// ReSharper restore InconsistentNaming
