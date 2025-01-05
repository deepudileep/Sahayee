using MongoDB.Bson;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class JobApplicationDetailsViewModel
    {
        public List<JobApplicationWithDetails> JobApplicationWithDetails { get; set; }
        public List<Positions> Position { get; set; }
        public List<Locations> Location { get; set; }
        public List<Institution> Institutions { get; set; }
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
        public JobDetails JobDetails { get; set; } // Nested class for job details
        public UserDetails UserDetails { get; set; } // Nested class for user details
    }

    public class JobDetails
    {
        public ObjectId Id { get; set; } // MongoDB Id
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Institution { get; set; }
        public string Location { get; set; }
        public string ContactEmail { get; set; }
        public string Description { get; set; }
    }

    public class UserDetails
    {
        public ObjectId Id { get; set; } // MongoDB Id
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public UserType UserType { get; set; } // Assuming UserType is another class or enum
    }


}
