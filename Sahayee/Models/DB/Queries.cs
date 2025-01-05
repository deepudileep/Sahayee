using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Sahayee.Models.DB
{
    public class Queries
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Profession { get; set; }
        public string Country { get; set; }
        public string Message { get; set; }
    }
}
