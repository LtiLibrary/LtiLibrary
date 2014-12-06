using LtiLibrary.Common;

namespace LtiLibrary.Lti1
{
    public enum Role
    {
        [Urn("urn:lti:role:ims/lis/Administrator")]
        Administrator,
        [Urn("urn:lti:role:ims/lis/ContentDeveloper")]
        ContentDeveloper,
        [Urn("urn:lti:role:ims/lis/Instructor")]
        Instructor,
        [Urn("urn:lti:role:ims/lis/Learner")]
        Learner,
        [Urn("urn:lti:role:ims/lis/Mentor")]
        Mentor,
        [Urn("urn:lti:role:ims/lis/TeachingAssistant")]
        TeachingAssistant
    }
}
