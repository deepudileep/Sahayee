using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class JobFilterViewModel
    {
        public List<Positions> Position { get; set; }
        public List<Locations> Location { get; set; }
        public List<Institution> Institutions { get; set; }
        public List<Jobs> Jobs { get; set; } = new List<Jobs>();
    }

    public class Positions
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

  
}
