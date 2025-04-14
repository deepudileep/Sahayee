using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class CourseFilterViewModel
    {
        
        public List<Course> Courses { get; set; } = new List<Course>();

        public List<Categories> Categories { get; set; }
        public List<CLocations> CourseType { get; set; }
        public List<Organization> Institutions { get; set; }
        public string CategoryId { get; set; }
        public string CourseTypeId { get; set; }
        public string InstitutionsId { get; set; }
        public string InstitutionsName { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class CourseHomeViewModel
    {
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<News> News { get; set; }
        public List<Organization> Organization { get; set; }
        public Queries Query { get; set; }
        public List<Categories> Categories { get; set; }

    }

}
