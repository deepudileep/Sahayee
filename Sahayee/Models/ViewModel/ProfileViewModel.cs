using Microsoft.AspNetCore.Mvc;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public JobCounts JobCounts { get; set; }
        public List<News> News { get; set; }

    }
}
