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
    public class QueriesController : Controller
    {
        private readonly ILogger<QueriesController> _logger;
        private readonly MongoDbService<Queries> _mongoDbService;

        public QueriesController(ILogger<QueriesController> logger, MongoDbService<Queries> mongoDbService)
        {
            _logger = logger;
            _mongoDbService = mongoDbService;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var filterCriteria = new Dictionary<string, string>
                {
                    { "Type", "all" },
                };
            var queries = _mongoDbService.ApplyFilters(filterCriteria);

            var viewModel = new QueriesFilterViewModel
            {
                Type = StaticData.GetQueriesType(),
                Queries = queries
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult FilterQueries(string type = "all")
        {
            var filterCriteria = new Dictionary<string, string>
                    {
                        { "Profession", type },
                    };
            var jobs = _mongoDbService.ApplyFilters(filterCriteria);

            return PartialView("_QueriesTablePartial", jobs);
        }

       
    }
}
