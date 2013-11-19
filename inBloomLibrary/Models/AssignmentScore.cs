namespace inBloomLibrary.Models
{
    public class AssignmentScore
    {
        public string TenantId { get; set; }
        public string GradebookEntryId { get; set; }
        public string StudentId { get; set; }
        public int NumericGradeEarned { get; set; }
    }
}
