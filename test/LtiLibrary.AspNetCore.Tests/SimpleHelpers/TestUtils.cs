using System.IO;

namespace LtiLibrary.AspNetCore.Tests.SimpleHelpers
{
    internal static class TestUtils
    {
        public static string LoadReferenceJsonFile(string refJsonName)
        {
            return File.ReadAllText("ReferenceJson/" + refJsonName + ".json");
        }

        public static string LoadReferenceTextFile(string filename)
        {
            return File.ReadAllText("ReferenceText/" + filename + ".txt");
        }
    }
}
