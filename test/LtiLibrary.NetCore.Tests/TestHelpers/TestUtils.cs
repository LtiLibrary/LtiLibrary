using System.IO;

namespace LtiLibrary.NetCore.Tests.TestHelpers
{
    internal static class TestUtils
    {
        public static string LoadReferenceJsonFile(string refJsonName)
        {
            return File.ReadAllText("ReferenceJson/" + refJsonName + ".json");
        }
    }
}
