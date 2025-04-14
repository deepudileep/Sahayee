using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class JobFilterViewModel
    {
        public List<CommonList>? Locations { get; set; }
        public List<CommonList>? Companies { get; set; }
        public List<JobTypes>? JobTypes { get; set; }
        public List<Categories>? JobCategories { get; set; }
        public List<Jobs> Jobs { get; set; } = new List<Jobs>();
    }

    public class Positions
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

  
}
