using MongoDB.Bson;

namespace Sahayee.Models.DB
{
    public class Organization
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Country { get; set; }
        public string ImagePath { get; set; }
        public DateTime LastModified { get; set; }
        public string Overview { get; set; } = string.Empty;
        public string AboutUs { get; set; } = string.Empty;
    }
}
