using MongoDB.Bson;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class JobApplicationDetailsViewModel
    {
        public List<JobApplicationWithDetails> JobApplicationWithDetails { get; set; }
        public List<CommonList>? Locations { get; set; }
        public List<CommonList>? Companies { get; set; }
        public List<JobTypes>? JobTypes { get; set; }
        public List<Categories>? JobCategories { get; set; }
    }


    public class JobApplicationWithDetails
    {
        public ObjectId Id { get; set; } // MongoDB Id
        public ObjectId UserId { get; set; } // Matching type with the database field
        public ObjectId JobId { get; set; } // Matching type with the database field
        public string Status { get; set; }
        public string UserMessage { get; set; }
        public string FollowUpBy { get; set; }

        public DateTime AppliedOn { get; set; }
        public DateTime FollowUpDate { get; set; }
        public List<FollowUp> FollowUpHistory { get; set; }
        public Jobs JobDetails { get; set; } // Nested class for job details
        public User UserDetails { get; set; } // Nested class for user details
    }

}
