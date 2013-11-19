using System.IO;
using System.Runtime.Serialization.Json;

namespace inBloomLibrary
{
    /// <summary>
    /// Json stream helpers.
    /// </summary>
    static class JsonHelper
    {
        /// <summary>
        /// The deserialize.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <typeparam name="T">The type of the value to deserialize.</typeparam>
        /// <returns>
        /// The deserialized value.
        /// </returns>
        public static T Deserialize<T>(Stream stream) where T : class
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }

        public static void Serialize(Stream stream, object graph)
        {
            var serializer = new DataContractJsonSerializer(graph.GetType());
            serializer.WriteObject(stream, graph);
        }
    }
}
