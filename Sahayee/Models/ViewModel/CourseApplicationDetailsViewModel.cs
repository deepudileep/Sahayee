using MongoDB.Bson;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class CourseApplicationDetailsViewModel
    {
        public List<CourseApplicationWithDetails> CourseApplicationWithDetails { get; set; }
        public List<Categories> Categories { get; set; }
        public List<CLocations> Location { get; set; }
        public List<Institution> Institutions { get; set; }
    }

    public class CourseApplicationWithDetails
    {
        public ObjectId Id { get; set; } // MongoDB Id
        public ObjectId UserId { get; set; }
        public ObjectId CourseId { get; set; } // Change from string to ObjectId
        public string Status { get; set; }
        public string UserMessage { get; set; }
        public DateTime AppliedOn { get; set; }
        public DateTime FollowUpDate { get; set; }
        public string FollowUpBy { get; set; }
        public List<CourseFollowUp> FollowUpHistory { get; set; }
        public Course CourseDetails { get; set; }
        public UserDetails UserDetails { get; set; }
    }
    
    
}
