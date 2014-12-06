using LtiLibrary.Lti1;

namespace SimpleLti12.Models
{
    public class ToolModel
    {
        public string ConsumerSecret { get; set; }
        public LtiRequest LtiRequest { get; set; }
    }
}