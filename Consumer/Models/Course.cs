using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace Consumer.Models
{
    public class Course
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
        /// <param name="user">The User for which this is a course.</param>
        public Course(User user)
        {
            PluralizationService plurals =
                PluralizationService.CreateService(CultureInfo.CurrentCulture);

            Id = user.UserId.ToString();
            Label = string.IsNullOrEmpty(user.FullName) ?
                user.UserId.ToString() :
                user.FullName;
            Title = string.Format("{0}{1}",
                Label,
                plurals.IsPlural(Label) ? "' Class" : "'s Class"
                );
            LisType = "CourseSection";            

            // State, District and School are typical context attributes
            // in K-12 systems.
            State = user.State;
            District = user.District;
            School = user.School;
        }

        public string Id { get; private set; }
        public string Label { get; private set; }
        public string LisType { get; private set; }
        public string Title { get; private set; }

        public State State { get; private set; }
        public District District { get; private set; }
        public School School { get; private set; }
    }
}