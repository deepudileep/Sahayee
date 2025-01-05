using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class NewsFilterViewModel
    {
        public List<Country> Country { get; set; }
        public List<News> News { get; set; } = new List<News>();
    }

    public class Country
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class NewsType
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

}
