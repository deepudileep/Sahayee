using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using MongoDB.Bson;
using Sahayee.Helper;
using Sahayee.Models;
using Sahayee.Models.DB;
using Sahayee.Models.ViewModel;
using Sahayee.Repository;
using System.Diagnostics;
using System.Security.Claims;

namespace Sahayee.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoDbService<User> _mongoDbService;
        private readonly MongoDbService<Queries> _mongoDbQueriesService;
        private readonly MongoDbService<News> _mongoDbServiceNews;
        private readonly MongoDbService<Organization> _mongoDbServiceOrganization;

        public HomeController(ILogger<HomeController> logger, MongoDbService<User> mongoDbService,
            MongoDbService<Queries> mongoDbQueriesService, MongoDbService<News> mongoDbServiceNews, MongoDbService<Organization> mongoDbServiceOrganization)
        {
            _logger = logger;
            _mongoDbService = mongoDbService;
            _mongoDbQueriesService = mongoDbQueriesService;
            _mongoDbServiceNews = mongoDbServiceNews;
            _mongoDbServiceOrganization = mongoDbServiceOrganization;
        }

        public IActionResult Index()
        {
            HomeViewModels homeViewModels = new HomeViewModels();
            homeViewModels.Query = new Queries();
            homeViewModels.News = _mongoDbServiceNews.Get()
      .OrderByDescending(news => news.NewsDate) // Order by the latest entries
      .Take(4) // Take the latest 6 entries
      .ToList();
            homeViewModels.Organization = _mongoDbServiceOrganization.Get().Take(8).ToList();
            return View(homeViewModels);
        }

        public IActionResult Contact()
        {
            return View();
        }


        [HttpPost]
        public IActionResult SaveUserQuery(string name, string email, string contact, string profession, string country, string message)
        {
            // Process the form data (save to database, send email, etc.)
            Queries queries = new Queries();
            queries.Name = name;
            queries.Email = email;
            queries.Contact = contact;
            queries.Profession = profession;
            queries.Country = country;
            queries.Message = message;
            string userId = string.Empty;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            try
            {
                if (!String.IsNullOrEmpty(userId))
                    queries.UserId = userId;
                else
                    queries.UserId = null;

                queries.Id = ObjectId.GenerateNewId().ToString();
                _mongoDbQueriesService.Insert(queries);
                TempData["AssistanceMessage"] = "Your request submitted successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["AssistanceMessage"] = "Spmething went wrong with your submission. please try it later.";
                return RedirectToAction("Index", "Home");
            }
            // Return a response (JSON, or just a success message)

        }

        public async Task<IActionResult> AdminDashboard()
        {
            var result = await _mongoDbService.GetAdminDashCountAsync();
            return View(result);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}