using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class CandidatesFilterViewModel
    {
        public List<CandidateViewModel> Candidates { get; set; }
        public List<Positions> Positions { get; set; }
        public string Name { get; set; }
    }

}
