using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class JobsViewModel
    {
        public string? Id { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Institution { get; set; }
        public string Location { get; set; }
        public string ContactEmail { get; set; }
        public string Description { get; set; }

        public List<Positions>? Positions { get; set; }
        public List<Locations>? Locations { get; set; }
        public List<Institution>? Institutions { get; set; }
    }
}
