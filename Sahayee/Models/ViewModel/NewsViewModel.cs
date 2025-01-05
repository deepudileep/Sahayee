using MongoDB.Bson;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class NewsViewModel
    {
        public string? Id { get; set; }      
        public string TypeId { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public DateTime NewsDate { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }

        public List<Country>? Countries { get; set; }
        public List<NewsType>? Type { get; set; }
    }
}
