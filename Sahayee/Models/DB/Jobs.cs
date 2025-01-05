using MongoDB.Bson;

namespace Sahayee.Models.DB
{
    public class Jobs
    {
        public ObjectId Id { get; set; }  // MongoDB _id field
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Institution { get; set; }
        public string Location { get; set; }
        public string ContactEmail { get; set; }
        public string Description { get; set; }
        public DateTime LastModified { get; set; }

    }
}
