using System;
using System.Collections.Generic;

namespace Consumer.Models
{
    public class CourseViewModel
    {
        public Course Course { get; set; }
        public IList<ScoredAssignmentModel> ScoredAssignments { get; set; }
    }

    public class CourseEnrollmentModel
    {
        public int CourseId { get; set; }
        public bool Enrolled { get; set; }
        public string UserId { get; set; }
    }

    public class CourseGradebookModel
    {
        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<ApplicationUser> EnrolledUsers { get; set; }
        public Dictionary<Tuple<string, int>, string> Scores { get; set; }
    }
}