using MongoDB.Bson;

namespace Sahayee.Models.DB
{
    public class News
    {
        public ObjectId Id { get; set; }  // MongoDB _id field
        public string TypeId { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public DateTime NewsDate { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
    }
}
