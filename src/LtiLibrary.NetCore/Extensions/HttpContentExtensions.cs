using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="HttpContent"/> type.
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Convert the JSON in HttpContent to an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="content">The JSON to deserialize.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public static async Task<T> ReadJsonAsObjectAsync<T>(this HttpContent content)
        {
            var value = await content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(value);
            return result;
        }
    }
}