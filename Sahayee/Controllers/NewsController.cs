using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Driver;
using Sahayee.Models.DB;
using Sahayee.Models.ViewModel;
using Sahayee.Repository;
using System.Security.Claims;

namespace Sahayee.Controllers
{
    public class NewsController : Controller
    {
        private readonly ILogger<NewsController> _logger;
        private readonly MongoDbService<News> _mongoDbService;

        public NewsController(ILogger<NewsController> logger, MongoDbService<News> mongoDbService)
        {
            _logger = logger;
            _mongoDbService = mongoDbService;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var filterCriteria = new Dictionary<string, string>
                {
                    { "Country", "all" },
                };
            var news = _mongoDbService.ApplyFilters(filterCriteria);

            var viewModel = new NewsFilterViewModel
            {
                Country = StaticData.GetCountries(),
                News = news
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult FilterNews(string country = "all")
        {
            var filterCriteria = new Dictionary<string, string>
                    {
                        { "Country", country },
                    };
            var jobs = _mongoDbService.ApplyFilters(filterCriteria);

            return PartialView("_NewsTablePartial", jobs);
        }

        [HttpGet]
        public IActionResult EditNews(string id)
        {
            var job = _mongoDbService.GetById(ObjectId.Parse(id));
            NewsViewModel jobsViewModel = new NewsViewModel();
            jobsViewModel.Countries = StaticData.GetCountries();
            jobsViewModel.Type = StaticData.GetNewsType();
            jobsViewModel.Title = job.Title;
            jobsViewModel.NewsDate = job.NewsDate;
            jobsViewModel.TypeId = job.TypeId;
            jobsViewModel.Country = job.Country;
            jobsViewModel.Summary = job.Summary;
            jobsViewModel.Content = job.Content;
            jobsViewModel.Id = job.Id.ToString();
            return PartialView("_NewsAddPartial", jobsViewModel);
        }
        [HttpGet]
        public IActionResult DetailsPartial(string id)
        {
            var job = _mongoDbService.GetById(ObjectId.Parse(id));
            NewsViewModel jobsViewModel = new NewsViewModel();
            jobsViewModel.Countries = StaticData.GetCountries();
            jobsViewModel.Type = StaticData.GetNewsType();
            jobsViewModel.Title = job.Title;
            jobsViewModel.NewsDate = job.NewsDate;
            jobsViewModel.TypeId = job.TypeId;
            jobsViewModel.Country = job.Country;
            jobsViewModel.Summary = job.Summary;
            jobsViewModel.Content = job.Content;
            jobsViewModel.Id = job.Id.ToString();
            return PartialView("_NewsDetailsPartial", jobsViewModel);
        }
        [HttpGet]
        public IActionResult AddNews()
        {
            NewsViewModel jobsViewModel = new NewsViewModel();
            jobsViewModel.Countries = StaticData.GetCountries();
            jobsViewModel.Type = StaticData.GetNewsType();
            return PartialView("_NewsAddPartial", jobsViewModel);
        }
        [HttpPost]
        public IActionResult DeleteNews(string id)
        {
            _mongoDbService.DeleteById(ObjectId.Parse(id));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SaveNews(NewsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var job = new News
                {
                    Title = model.Title,
                    TypeId = model.TypeId,
                    Country = model.Country,
                    NewsDate = model.NewsDate,
                    Summary = model.Summary,
                    Content = model.Content,
                    Id = ObjectId.GenerateNewId()
                };
                if (String.IsNullOrEmpty(model.Id))
                    _mongoDbService.Insert(job);
                else
                {
                    job.Id = ObjectId.Parse(model.Id);
                    _mongoDbService.UpdateById(ObjectId.Parse(model.Id), job);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
