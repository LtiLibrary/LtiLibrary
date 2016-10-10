using System;
using System.IO;
using System.Reflection;

namespace LtiLibrary.Core.Tests.TestHelpers
{
    internal static class TestUtils
    {
        public static string LoadReferenceJsonFile(string refJsonName)
        {
            #if NetCore
            return File.ReadAllText("ReferenceJson/" + refJsonName + ".json");
            #else
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "LtiLibrary.Core.Tests.ReferenceJson." + refJsonName + ".json";
            string content;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new Exception("Missing reference json: " + refJsonName);

                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }

            return content;
            #endif
        }
    }
}
