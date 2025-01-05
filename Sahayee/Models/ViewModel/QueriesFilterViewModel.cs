using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class QueriesFilterViewModel
    {
        public List<QueriesType> Type { get; set; }
        public List<Queries> Queries { get; set; } = new List<Queries>();
    }

    public class QueriesType
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
