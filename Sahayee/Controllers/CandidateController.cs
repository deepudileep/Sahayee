using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Driver;
using Sahayee.Models.DB;
using Sahayee.Models.ViewModel;
using Sahayee.Repository;
using System.Security.Claims;
using System.Xml.Linq;

namespace Sahayee.Controllers
{
    public class CandidateController : Controller
    {
        private readonly ILogger<CandidateController> _logger;
        private readonly MongoDbService<User> _mongoDbService;

        public CandidateController(ILogger<CandidateController> logger, MongoDbService<User> mongoDbService)
        {
            _logger = logger;
            _mongoDbService = mongoDbService;
        }


        [HttpGet]
        public IActionResult Index()
        {
            List<CandidateViewModel> candidateList = new List<CandidateViewModel>();

            var users = _mongoDbService.GetCandidates(string.Empty, string.Empty);
            foreach (var item in users)
            {
                var stats = _mongoDbService.GetUserStatistics(item.Id.ToString());

                CandidateViewModel candidateViewModel = new CandidateViewModel();
                candidateViewModel.Id = item.Id.ToString();
                candidateViewModel.Name = item.FirstName + " " + item.LastName;
                candidateViewModel.Profession = item.Occupation;
                candidateViewModel.JobsApplied = stats.JobCount;
                candidateViewModel.CoursesApplied = stats.CourseCount;
                candidateViewModel.Queries = stats.QueryCount;
                candidateViewModel.RegisteredOn = DateTime.Now;
                candidateViewModel.ProfileStatus = "";
                candidateViewModel.LastFollowUp = null;
                candidateViewModel.FollowUpOn = null;
                candidateViewModel.FollowUpDoneBy = null;
                candidateList.Add(candidateViewModel);
            };


            var viewModel = new CandidatesFilterViewModel
            {
                Candidates = candidateList,
                Name = string.Empty,
                Positions = StaticData.GetPositions()
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult FilterQueries(string name = "", string occupation = "")
        {
            List<CandidateViewModel> candidateList = new List<CandidateViewModel>();
            var users = _mongoDbService.GetCandidates(name, occupation);
            foreach (var item in users)
            {
                var stats = _mongoDbService.GetUserStatistics(item.Id.ToString());

                CandidateViewModel candidateViewModel = new CandidateViewModel();
                candidateViewModel.Id = item.Id.ToString();
                candidateViewModel.Name = item.FirstName + " " + item.LastName;
                candidateViewModel.Profession = item.Occupation;
                candidateViewModel.JobsApplied = stats.JobCount;
                candidateViewModel.CoursesApplied = stats.CourseCount;
                candidateViewModel.Queries = stats.QueryCount;
                candidateViewModel.RegisteredOn = DateTime.Now;
                candidateViewModel.ProfileStatus = "";
                candidateViewModel.LastFollowUp = null;
                candidateViewModel.FollowUpOn = null;
                candidateViewModel.FollowUpDoneBy = null;
                candidateList.Add(candidateViewModel);
            };
            return PartialView("_CandidatesTablePartial", candidateList);
        }

        [HttpGet]
        public IActionResult Details(string userId)
        {
            CandidateDetailsViewModel candidateViewModel = new CandidateDetailsViewModel();
            var item = _mongoDbService.GetById(ObjectId.Parse(userId));
            if (item != null)
            {
                candidateViewModel.User = item;
                candidateViewModel.UsersStatistics = _mongoDbService.GetUserStatistics(item.Id.ToString());
            }
            return View(candidateViewModel);
        }
    }
}
