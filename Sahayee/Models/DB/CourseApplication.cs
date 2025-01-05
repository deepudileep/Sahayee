using MongoDB.Bson;

namespace Sahayee.Models.DB
{
    public class CourseApplication
    {
        public ObjectId Id { get; set; } // MongoDB Id
        public string UserId { get; set; } // Reference to the user
        public string CourseId { get; set; } // Reference to the job
        public string Status { get; set; } // Status of application (e.g., "Applied", "Reviewed", "Rejected")
        public string UserMessage { get; set; } // Custom message or static description
        public DateTime AppliedOn { get; set; } = DateTime.Now; // Timestamp of application

        public DateTime FollowUpDate { get; set; }
        public string FollowUpBy { get; set; }
        public List<CourseFollowUp> FollowUpHistory { get; set; }

        public DateTime LastModified { get; set; }
    }

    public class CourseFollowUp
    {
        public DateTime Date { get; set; }
        public string ApplicationStatus { get; set; }
        public string AdminNotes { get; set; }
        public string DoneBy { get; set; }
    }
}
