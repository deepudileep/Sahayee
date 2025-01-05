using MongoDB.Bson;

namespace Sahayee.Models.DB
{
    public class Course
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Institution { get; set; }
        public string Location { get; set; }
        public string Duration { get; set; }
        public string Summary { get; set; }
        public DateTime StartDate { get; set; }
        public string Trainer { get; set; }
        public DateTime LastModified { get; set; }
    }
}
