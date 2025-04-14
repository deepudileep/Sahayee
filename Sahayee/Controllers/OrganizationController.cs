using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using MongoDB.Bson;
using Sahayee.Models.DB;
using Sahayee.Models.ViewModel;
using Sahayee.Repository;

namespace Sahayee.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly ILogger<OrganizationController> _logger;
        private readonly MongoDbService<Organization> _mongoDbService;
        private readonly MongoDbService<CourseApplication> _mongoDbServiceCA;
        private readonly MongoDbService<News> _mongoDbServiceNews;
        private readonly MongoDbService<Course> _mongoDbServiceCourse;

        public OrganizationController(ILogger<OrganizationController> logger, MongoDbService<Organization> mongoDbService,
            MongoDbService<CourseApplication> mongoDbServiceCA, MongoDbService<News> mongoDbServiceNews, MongoDbService<Course> mongoDbServiceCourse)
        {
            _logger = logger;
            _mongoDbService = mongoDbService;
            _mongoDbServiceCA = mongoDbServiceCA;
            _mongoDbServiceNews = mongoDbServiceNews;
            _mongoDbServiceCourse = mongoDbServiceCourse;
        }
        public IActionResult Index()
        {
            List<Organization> organizationViewModel = new List<Organization>();
             organizationViewModel = _mongoDbService.Get().ToList();
            return View(organizationViewModel);
        }
        public IActionResult Details(string id)
        {
            OrganizationDetailsViewModel organizationViewModel = new OrganizationDetailsViewModel();
            organizationViewModel.Organization = _mongoDbService.GetById(ObjectId.Parse(id));
            var filterCriteria = new Dictionary<string, string>
                {               
                    { "Institution", id },
                };
            organizationViewModel.Courses = _mongoDbServiceCourse.ApplyFilters(filterCriteria);

            return View(organizationViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Create(string id)
        {
            OrganizationViewModel organizationViewModel = new OrganizationViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                var org = _mongoDbService.GetById(ObjectId.Parse(id));
                organizationViewModel = new OrganizationViewModel
                {
                    Id = org.Id.ToString(),
                    Name = org.Name,
                    Website = org.Website,
                    Email = org.Email,
                    Contact = org.Contact,
                    Address = org.Address,
                    Country = org.Country,
                    AboutUs = org.AboutUs,
                    LastModified = DateTime.Now,
                    Overview = org.Overview,
                    ImagePath = org.ImagePath,
                };
            }
           
            return View(organizationViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(OrganizationViewModel viewModel, IFormFile? imagePathFile)
        {
            // Handle file uploads (if any)
            if (imagePathFile != null)
            {
                var photoPath = Path.Combine(Guid.NewGuid() + imagePathFile.FileName);
                var logoPath = Path.Combine("wwwroot/uploads", photoPath);
                using (var stream = new FileStream(logoPath, FileMode.Create))
                {
                    await imagePathFile.CopyToAsync(stream);
                }
                viewModel.ImagePath = photoPath;
            }
            var org = new Organization
            {
                Id = string.IsNullOrEmpty(viewModel.Id) ? MongoDB.Bson.ObjectId.GenerateNewId() : ObjectId.Parse(viewModel.Id),
                Name = viewModel.Name,
                Website = viewModel.Website,
                Email = viewModel.Email,
                Contact = viewModel.Contact,
                Address = viewModel.Address,
                Country = viewModel.Country,
                AboutUs = viewModel.AboutUs,
                LastModified = DateTime.Now,
                Overview = viewModel.Overview,
                ImagePath = viewModel.ImagePath,
            };
            if (string.IsNullOrEmpty(viewModel.Id))
            {
                // Save the model to MongoDB

                _mongoDbService.Insert(org);
            }
            else
            {
                _mongoDbService.UpdateById(org.Id, org);
            }
            return RedirectToAction("Index");
        }
    }
}
