using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class HomeViewModels
    {
        public JobCounts JobCounts { get; set; }
        public List<News> News { get; set; }
        public List<Organization> Organization { get; set; }

        public Queries Query { get; set; }
        public RegistrationViewModel Registration { get; set; }
    }

    public class BlogViewModel
    {       
        public List<News> News { get; set; }
        public News Selected { get; set; }
        public List<News> PopularNews { get; set; }
    }
}
