namespace inBloomLibrary.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string CustomParameters { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}