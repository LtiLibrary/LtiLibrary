using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable InconsistentNaming
namespace LtiLibrary.NetCore.Lti.v1
{
    /// <summary>
    /// Represents IMS DocumentTarget values.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DocumentTarget
    {
        /// <summary>
        /// The TP page is inserted directly into the TC page; this option is not expected 
        /// to be a common use case but could be used, for example, when the launch request 
        /// is performed on behalf of the user by the TC (server-to-server) and the response 
        /// rendered within its page (e.g. within a portal-like interface).
        /// </summary>
        embed,
        /// <summary>
        /// Opened in the same frame as the resource link.
        /// </summary>
        frame,
        /// <summary>
        /// Opened within an iframe placed inside the same page/frame as the resource link.
        /// </summary>
        iframe,
        /// <summary>
        /// The item is not intended for display but for storing or processing (for example, 
        /// an assignment submission may just be stored without a link to it being added to the course).
        /// Only applies to Content-Item Message.
        /// </summary>
        none,
        /// <summary>
        /// Opened over the top of the page where the link exists (for example, using a lightbox).
        /// </summary>
        overlay,
        /// <summary>
        /// Opened in a popup window.
        /// </summary>
        popup,
        /// <summary>
        /// Opened in a new window (or tab).
        /// </summary>
        window
    }
}
// ReSharper restore InconsistentNaming
