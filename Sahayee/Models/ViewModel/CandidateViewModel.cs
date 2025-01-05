using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class CandidateViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Profession { get; set; }
        public int JobsApplied { get; set; }
        public int CoursesApplied { get; set; }
        public int Queries { get; set; }
        public DateTime RegisteredOn { get; set; }
        public string ProfileStatus { get; set; }
        public DateTime? LastFollowUp { get; set; }
        public DateTime? FollowUpOn { get; set; }
        public string FollowUpDoneBy { get; set; }
    }
    public class CandidateDetailsViewModel
    {
        public User User { get; set; }
        public UserStatistics UsersStatistics { get; set; }

    }
    public class UserStatistics
    {
        public string UserId { get; set; }
        public int JobCount { get; set; }
        public int CourseCount { get; set; }
        public int QueryCount { get; set; }
    }
}
