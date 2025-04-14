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
        public IActionResult Blog()
        {
            BlogViewModel homeViewModels = new BlogViewModel();
            homeViewModels.PopularNews = _mongoDbService.Get()
      .OrderByDescending(news => news.NewsDate) // Order by the latest entries
      .Take(4) // Take the latest 6 entries
      .ToList();
            homeViewModels.News = _mongoDbService.Get().ToList();
            return View(homeViewModels);
        }

        [HttpGet]
        public IActionResult BlogDetails(string id)
        {
            BlogViewModel homeViewModels = new BlogViewModel();
            homeViewModels.PopularNews = _mongoDbService.Get()
      .OrderByDescending(news => news.NewsDate) // Order by the latest entries
      .Take(4) // Take the latest 6 entries
      .ToList();
            homeViewModels.News = _mongoDbService.Get().ToList();
            homeViewModels.Selected = _mongoDbService.GetById(ObjectId.Parse(id));
            return View(homeViewModels);
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

        [HttpPost]
        public IActionResult DeleteNews(string id)
        {
            _mongoDbService.DeleteById(ObjectId.Parse(id));
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Create(string id)
        {
            NewsViewModel jobsViewModel = new NewsViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                var job = _mongoDbService.GetById(ObjectId.Parse(id));

                jobsViewModel.Countries = StaticData.GetCountries();
                jobsViewModel.Type = StaticData.GetNewsType();
                jobsViewModel.Title = job.Title;
                jobsViewModel.NewsDate = job.NewsDate;
                jobsViewModel.image = job.image;
                jobsViewModel.TypeId = job.TypeId;
                jobsViewModel.Country = job.Country;
                jobsViewModel.Summary = job.Summary;
                jobsViewModel.Content = job.Content;
                jobsViewModel.Id = job.Id.ToString();
            }
            jobsViewModel.Countries = StaticData.GetCountries();
            jobsViewModel.Type = StaticData.GetNewsType();
            return View(jobsViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(NewsViewModel model, IFormFile? image)
        {
            // Handle file uploads (if any)
            var job = new News
            {
                Title = model.Title,
                TypeId = model.TypeId,
                Country = model.Country,
                NewsDate = model.NewsDate,
                Summary = model.Summary,
                Content = model.Content,
            };
            if (image != null)
            {
                var imagePath = Path.Combine(Guid.NewGuid() + image.FileName);
                var logoPath = Path.Combine("wwwroot/uploads", imagePath);
                using (var stream = new FileStream(logoPath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                job.image = imagePath;
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                // Save the model to MongoDB

                job.Id = ObjectId.GenerateNewId();
                _mongoDbService.Insert(job);
            }
            else
            {
                job.Id = ObjectId.Parse(model.Id);
                _mongoDbService.UpdateById(job.Id, job);
            }
            return RedirectToAction("Index");
        }

    }
}
