using LtiLibrary.NetCore.Outcomes.v2;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v2
{
    public class OutcomesDataFixture
    {
        public static LineItem LineItem;
        public static LisResult Result;
        public static string ContextId = "course-1";
        public static string LineItemId = "lineitem-1";
        public static string ResultId = "result-1";

        public void InitializeData()
        {
            LineItem = null;
            Result = null;
        }
    }
}
