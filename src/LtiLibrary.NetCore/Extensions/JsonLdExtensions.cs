using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Extensions
{
    /// <summary>
    /// Extension methods for JSON-LD objects.
    /// </summary>
    public static class JsonLdExtensions
    {
        /// <summary>
        /// Serializes the specified object to a JSON string using default contract resolver.
        /// </summary>
        /// <remarks>
        /// This should only be used for inbound requests for pretty display. Use ToJsonLdString
        /// to format outbound requests.
        /// </remarks>
        public static string ToJsonString<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using a JSON-LD contract resolver.
        /// </summary>
        /// <remarks>
        /// This should only be used for outbound requests. Use ToJsonString to format inbound
        /// requests for pretty display.
        /// </remarks>
        public static string ToJsonLdString<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ContractResolver = new JsonLdObjectContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Deserializes the JSON response to the specified .NET type.
        /// <param name="response">The HttpResponseMessage to deserialize.</param>
        /// <param name="deserializationErrorHandler">The EventHandler to call if there is a deserialization error. Defaults to null.</param>
        /// </summary>
        public static async Task<T> DeserializeJsonObjectAsync<T>(this HttpResponseMessage response, 
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler = null)
        {
            try
            {
            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                if (stream == null)
                {
                    return default(T);
                }

                using (var reader = new StreamReader(stream))
                {
                    var body = await reader.ReadToEndAsync().ConfigureAwait(false);

                    return JsonConvert.DeserializeObject<T>(body, new JsonSerializerSettings { Error = deserializationErrorHandler });
                }
            }

            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}