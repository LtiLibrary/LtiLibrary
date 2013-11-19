using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Consumer.Models
{
    /// <summary>
    /// Represents a course (context in LTI). In this sample app, every
    /// user automatically gets their own "course" or list of assignments.
    /// </summary>
    /// <remarks>
    /// The LIS type of a course can be "CourseTemplate", "CourseOffering",
    /// "CourseSection", or "Group". In K-12, a CourseTemplate is roughly equivalent
    /// to a curriculum map. For example, your district may have identified a
    /// a collection of content and tools that meet the state standards
    /// for 4th grade math. If that collection were captured in a Learning
    /// Object Repository (LOR), then when the user launched one of the learning 
    /// objects while reviewing the curriculum map, the LTI request would
    /// use "CourseTemplate".
    ///
    /// CourseOffering is roughly equivalent to a curriculum plan. For example, 
    /// your school may have taken the curriculum map and created a teaching plan
    /// that is more specific to the children in your school (e.g. perhaps there
    /// are two plans: one for native english speakers and one for english language
    /// learners). As before, if the plan is captured in a LOR and the user launched
    /// the learning object while reviewing the plan in the LOR, then the LOR
    /// should send "CourseOffering".
    ///
    /// CourseSection is equivalent to an actual class like "Mrs. Taylor's Homeroom"
    /// of "Mr. Bill's Science Class". This is closest to what this sample app
    /// does, so that is what I use here.
    /// </remarks>
    public class Course
    {
        public int CourseId { get; set; }
        public virtual User Instructor { get; set; }
        public string Name { get; set; }
        public string inBloomSectionId { get; set; }
        public virtual State State { get; set; }
        public virtual District District { get; set; }
        public virtual School School { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<User> EnrolledUsers { get; set; }

        public Course()
        {
            Assignments = new HashSet<Assignment>();
            EnrolledUsers = new HashSet<User>();
        }

        public string Title
        {
            get
            {
                return string.IsNullOrEmpty(inBloomSectionId) ? Name : string.Format("SLC: {0}", Name);
            }
        }
    }

    public class CreateEditCourseModel
    {
        public int CourseId { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "State")]
        public string StateId { get; set; }

        [Display(Name = "District")]
        public string DistrictId { get; set; }

        [Display(Name = "School")]
        public string SchoolId { get; set; }

        public SelectList AvailableDistricts { get; set; }
        public SelectList AvailableSchools { get; set; }
        public SelectList AvailableStates { get; set; }

        public CreateEditCourseModel() { }
        public CreateEditCourseModel(Course course)
        {
            CourseId = course.CourseId;
            DistrictId = course.District == null ? null : course.District.DistrictId;
            Name = course.Name;
            SchoolId = course.School == null ? null : course.School.SchoolId;
            StateId = course.State == null ? null : course.State.StateId;
        }

        public string Title
        {
            get
            {
                return Name;
            }
        }
    }

    public class CourseEnrollmentModel
    {
        public int CourseId { get; set; }
        public bool Enrolled { get; set; }
        public int UserId { get; set; }
    }

    public class CourseGradebookModel
    {
        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<User> EnrolledUsers { get; set; }
        public Dictionary<Tuple<int, int>, string> Scores { get; set; }
    }

    public class ImportInBloomSectionModel
    {
        public int? CourseId { get; set; }
        public string Name { get; set; }
        public string SectionId { get; set; }
    }
}