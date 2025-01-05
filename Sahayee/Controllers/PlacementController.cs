using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using MongoDB.Bson;
using NuGet.Protocol.Core.Types;
using Sahayee.Helper;
using Sahayee.Models;
using Sahayee.Models.DB;
using Sahayee.Models.ViewModel;
using Sahayee.Repository;
using System.Diagnostics;
using System.Security.Claims;

namespace Sahayee.Controllers
{
    public class PlacementController : Controller
    {
        private readonly ILogger<PlacementController> _logger;
        private readonly MongoDbService<Jobs> _mongoDbServiceJobs;
        private readonly MongoDbService<News> _mongoDbServiceNews;
        private readonly MongoDbService<Queries> _mongoDbServiceQueries;
        private readonly MongoDbService<User> _mongoDbServiceUser;

        public PlacementController(ILogger<PlacementController> logger, MongoDbService<Jobs> mongoDbServiceJobs,
            MongoDbService<News> mongoDbServiceNews, MongoDbService<Queries> mongoDbServiceQueries, MongoDbService<User> mongoDbServiceUser)
        {
            _logger = logger;
            _mongoDbServiceJobs = mongoDbServiceJobs;
            _mongoDbServiceNews = mongoDbServiceNews;
            _mongoDbServiceQueries = mongoDbServiceQueries;
            _mongoDbServiceUser = mongoDbServiceUser;
        }

        public IActionResult Career()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            PlacementsViewModels placementsViewModels = new PlacementsViewModels();
            placementsViewModels.News = _mongoDbServiceNews.Get().ToList();
            JobCounts jobCounts = new JobCounts();
            jobCounts.Category = await _mongoDbServiceJobs.CountDistinctValuesAsync("Department");
            jobCounts.Hospital = await _mongoDbServiceJobs.CountDistinctValuesAsync("Institution");
            jobCounts.Country = await _mongoDbServiceJobs.CountDistinctValuesAsync("Location");
            placementsViewModels.JobCounts = jobCounts;
            string userId = string.Empty;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = _mongoDbServiceUser.GetById(ObjectId.Parse(userId));
                placementsViewModels.Query = new Queries()
                {
                    Name = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    Contact = user.PhoneNumber
                };
            }
            else
                placementsViewModels.Query = new Queries();
            return View(placementsViewModels);
        }
        public IActionResult PlacementsAuthorised()
        {
            return View();
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

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] Queries form)
        {

            PlacementsViewModels placementsViewModels = new PlacementsViewModels();
            placementsViewModels.News = _mongoDbServiceNews.Get().ToList();
            JobCounts jobCounts = new JobCounts();
            jobCounts.Category = await _mongoDbServiceJobs.CountDistinctValuesAsync("Department");
            jobCounts.Hospital = await _mongoDbServiceJobs.CountDistinctValuesAsync("Institution");
            jobCounts.Country = await _mongoDbServiceJobs.CountDistinctValuesAsync("Location");
            placementsViewModels.JobCounts = jobCounts;
            string userId = string.Empty;
            placementsViewModels.Query = form;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            try
            {
                if (!String.IsNullOrEmpty(userId))
                    form.UserId = userId;
                else
                    form.UserId = null;

                form.Id = ObjectId.GenerateNewId().ToString();
                _mongoDbServiceQueries.Insert(form);
                ViewBag.Message = "Query submitted successfully!";
                return View(placementsViewModels); // Replace "YourViewName" with the actual view name

            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while submitting your query. Please try again.";
                return View();  // Replace "YourViewName" with the actual view name
            }
        }
    }
}