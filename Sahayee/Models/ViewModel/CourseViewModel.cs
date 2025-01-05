using MongoDB.Bson;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class CourseViewModel
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Institution { get; set; }
        public string Location { get; set; }
        public string Duration { get; set; }
        public string Summary { get; set; }
        public DateTime StartDate { get; set; }
        public string Trainer { get; set; }

        public List<Categories>? Categories { get; set; }
        public List<Institution>? Institutions { get; set; }
        public List<CLocations>? Locations { get; set; }
    }
}
