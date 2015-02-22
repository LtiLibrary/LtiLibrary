using System;
using System.IO;
using System.Net;
using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.Core.Extensions
{
    public static class JsonLdExtensions
    {
        /// <summary>
        /// Serializes the specified object to a JSON string using a JSON-LD contract resolver.
        /// </summary>
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
        /// </summary>
        public static T DeserializeObject<T>(this HttpWebResponse response)
        {
            try
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        return default(T);
                    }

                    using (var reader = new StreamReader(stream))
                    {
                        var body = reader.ReadToEnd();
                        return JsonConvert.DeserializeObject<T>(body);
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
