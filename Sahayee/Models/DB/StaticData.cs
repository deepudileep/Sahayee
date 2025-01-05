using Sahayee.Models.ViewModel;

namespace Sahayee.Models.DB
{
    public static class StaticData
    {
        // Method to get positions
        public static List<Positions> GetPositions() => new List<Positions>
        {
            new Positions { Id = "all", Name = "All Positions" },
            new Positions { Id = "Doctor", Name = "Doctor" },
            new Positions { Id = "Nurse", Name = "Nurse" },
            new Positions { Id = "Technician", Name = "Technician" },
            new Positions { Id = "Administrator", Name = "Administrator" }
        };

        // Method to get categories
        public static List<Categories> GetCategories() => new List<Categories>
        {
            new Categories { Id = "all", Name = "All Categories" },
            new Categories { Id = "Healthcare", Name = "Healthcare" },
            new Categories { Id = "Management", Name = "Management" },
            new Categories { Id = "Technical", Name = "Technical" }
        };

        // Method to get locations
        public static List<Locations> GetLocations() => new List<Locations>
        {
            new Locations { Id = "all", Name = "All Locations" },
            new Locations { Id = "UK", Name = "UK" },
            new Locations { Id = "USA", Name = "USA" },
            new Locations { Id = "Canada", Name = "Canada" },
            new Locations { Id = "India", Name = "India" }
        };

        // Method to get CLocations
        public static List<CLocations> GetCLocations() => new List<CLocations>
        {
            new CLocations { Id = "all", Name = "All Locations" },
            new CLocations { Id = "Onsite", Name = "Onsite" },
            new CLocations { Id = "Online", Name = "Online" },
        };
        public static List<Institution> GetInstitution() => new List<Institution>
        {
            new Institution { Id = "all", Name = "All Institution" },
            new Institution { Id = "Institution1", Name = "Institution 1" },
            new Institution { Id = "Institution2", Name = "Institution 2" }
        };

        public static List<Country> GetCountries() => new List<Country>
        {
            new Country { Id = "all", Name = "All" },
            new Country { Id = "USA", Name = "USA" },
            new Country { Id = "UK", Name = "UK" }
        };

        public static List<NewsType> GetNewsType() => new List<NewsType>
        {
            new NewsType { Id = "all", Name = "All " },
            new NewsType { Id = "Course", Name = "Course" },
            new NewsType { Id = "Job", Name = "Job" },
            new NewsType { Id = "General", Name = "General" }
        };
        public static List<QueriesType> GetQueriesType() => new List<QueriesType>
        {
            new QueriesType { Id = "all", Name = "All " },
            new QueriesType { Id = "Nurse", Name = "Nurse" },
            new QueriesType { Id = "Physician", Name = "Physician" },
            new QueriesType { Id = "Lab Technician", Name = "Lab Technician" },
            new QueriesType { Id = "Home Care Assistant", Name = "Home Care Assistant" },
            new QueriesType { Id = "General", Name = "General" }
        };


    }
}
