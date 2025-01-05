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

namespace Sahayee.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoDbService<User> _mongoDbService;

        public HomeController(ILogger<HomeController> logger, MongoDbService<User> mongoDbService)
        {
            _logger = logger;
            _mongoDbService = mongoDbService;
        }

        public IActionResult Index()
        {
            return View();
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