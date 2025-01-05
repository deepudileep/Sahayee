using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class CourseFilterViewModel
    {
        public List<Categories> Categories { get; set; }
        public List<CLocations> Location { get; set; }
        public List<Institution> Institutions { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }

}
