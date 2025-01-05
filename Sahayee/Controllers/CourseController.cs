using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Driver;
using Sahayee.Models.DB;
using Sahayee.Models.ViewModel;
using Sahayee.Repository;
using System.Security.Claims;

namespace Sahayee.Controllers
{
    public class CourseController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly MongoDbService<Course> _mongoDbService;
        private readonly MongoDbService<CourseApplication> _mongoDbServiceCA;

        public CourseController(ILogger<CourseController> logger, MongoDbService<Course> mongoDbService, MongoDbService<CourseApplication> mongoDbServiceCA)
        {
            _logger = logger;
            _mongoDbService = mongoDbService;
            _mongoDbServiceCA = mongoDbServiceCA;
        }


        [HttpGet]
        public IActionResult Courses()
        {
            var filterCriteria = new Dictionary<string, string>
                {
                    { "Category", "all" },
                    { "Location", "all" },
                    { "Institution", "all" },
                };
            var course = _mongoDbService.ApplyFilters(filterCriteria);

            var viewModel = new CourseFilterViewModel
            {
                Categories = StaticData.GetCategories(),
                Location = StaticData.GetCLocations(),
                Institutions = StaticData.GetInstitution(),
                Courses = course
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> MyCourses()
        {
            var userId = string.Empty;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            var result = await _mongoDbService.GetCoursesAppliedByUserAsync(userId);
            var viewModel = new CourseApplicationDetailsViewModel
            {
                Categories = StaticData.GetCategories(),
                Location = StaticData.GetCLocations(),
                Institutions = StaticData.GetInstitution(),
                CourseApplicationWithDetails = result
            };
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> DetailsView(string id)
        {
            var result = _mongoDbService.GetById(ObjectId.Parse(id));
            return View(result);
        }
        public async Task<IActionResult> AdminApplicantDetails(string courseId)
        {
            // Fetch job applications based on jobId if necessary
            var courseApplication = _mongoDbService.GetCoursesAppliedByCourseAsync(courseId).Result;
            if (courseApplication == null)
            {
                return NotFound();
            }

            return View(courseApplication);
        }

        [HttpPost]
        public async Task<IActionResult> SaveFollowUp([FromBody] FollowUpUpdateModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.ApplicationId))
            {
                TempData["Message"] = "Application ID is required.";
                return BadRequest(new { success = false, message = "Application ID is required." });
            }

            var inputModel = new CourseFollowUp()
            {
                AdminNotes = model.AdminNotes,
                ApplicationStatus = model.Status,
                Date = DateTime.Now,
                DoneBy = "Admin" // Replace with the actual user's name or ID
            };
            var filter = Builders<CourseApplication>.Filter.Eq(j => j.LastModified, inputModel.Date);
            _mongoDbServiceCA.UpdateLastModified(filter);
            try
            {
                var result = await _mongoDbService.UpdateFollowUpHistory(model.ApplicationId, inputModel);

                if (result.MatchedCount > 0)
                {
                    TempData["Message"] = "Application updated successfully!";
                    return Ok(new { success = true, message = "Follow-up history updated successfully." });
                }

                TempData["Message"] = "Failed to update the application.";
                return StatusCode(500, new { success = false, message = "Failed to update follow-up history." });
            }
            catch (Exception ex)
            {
                TempData["Message"] = "An error occurred while updating the application.";
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ApplyForCourse([FromForm] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Message"] = "Course ID is required.";
                return RedirectToAction("DetailsView", new { id = id }); // Redirect to the list view
            }
            var userId = string.Empty;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "Please login to apply for this position!";
                return RedirectToAction("DetailsView", new { id = id });
            }

            var filterCriteria = new Dictionary<string, string>
    {
        { "UserId", userId },
        { "CourseId", id }
    };

            var courses = _mongoDbServiceCA.ApplyFilters(filterCriteria);
            if (courses.Count > 0)
            {
                TempData["Message"] = "Already applied for this course!";
                return RedirectToAction("DetailsView", new { id = id });
            }
            var Cdate = DateTime.Now;
            var application = new CourseApplication
            {
                Id = ObjectId.GenerateNewId(),
                CourseId = id,
                UserId = userId,
                Status = "Applied",
                UserMessage = "",
                AppliedOn = DateTime.Now,
                FollowUpBy = "",
                FollowUpDate = Cdate,
                LastModified = DateTime.Now,
                FollowUpHistory = new List<CourseFollowUp>
                {
                    new CourseFollowUp
                    {
                        Date = Cdate,
                        ApplicationStatus = "Payment Pending",
                        AdminNotes = "Requested payment confirmation.",
                        DoneBy = "Admin"
                    },
                }
            };

            _mongoDbServiceCA.Insert(application);

            TempData["Message"] = "Application submitted successfully!";
            return RedirectToAction("DetailsView", new { id = id });
        }

        [HttpGet]
        public IActionResult ReloadCourse(string category = "all", string location = "all", string institution = "all")
        {
            var filterCriteria = new Dictionary<string, string>
                {
                    { "Category", category },
                    { "Location", location },
                    { "Institution", institution },
                };
            var course = _mongoDbService.ApplyFilters(filterCriteria);
            var viewModel = new CourseFilterViewModel
            {
                Categories = StaticData.GetCategories(),
                Location = StaticData.GetCLocations(),
                Institutions = StaticData.GetInstitution(),
                Courses = course
            };

            return PartialView("_CoursesPartial", viewModel);
        }


        [HttpGet]
        public IActionResult Index()
        {
            var filterCriteria = new Dictionary<string, string>
                {
                    { "Category", "all" },
                    { "Location", "all" },
                    { "Institution", "all" },
                };
            var course = _mongoDbService.ApplyFilters(filterCriteria);

            var viewModel = new CourseFilterViewModel
            {
                Categories = StaticData.GetCategories(),
                Location = StaticData.GetCLocations(),
                Institutions = StaticData.GetInstitution(),
                Courses = course
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult FilterCourse(string category = "all", string location = "all", string institution = "all")
        {
            var filterCriteria = new Dictionary<string, string>
                {
                    { "Category", category },
                    { "Location", location },
                    { "Institution", institution },
                };
            var course = _mongoDbService.ApplyFilters(filterCriteria);


            return PartialView("_CoursesTablePartial", course);
        }
        [HttpGet]
        public IActionResult EditCourse(string id)
        {
            var course = _mongoDbService.GetById(ObjectId.Parse(id));
            CourseViewModel courseViewModel = new CourseViewModel();
            courseViewModel.Id = course.Id.ToString();
            courseViewModel.Institution = course.Institution;
            courseViewModel.Duration = course.Duration;
            courseViewModel.Summary = course.Summary;
            courseViewModel.Category = course.Category;
            courseViewModel.Name = course.Name;
            courseViewModel.Location = course.Location;
            courseViewModel.Trainer = course.Trainer;
            courseViewModel.StartDate = course.StartDate;

            courseViewModel.Locations = StaticData.GetCLocations();
            courseViewModel.Categories = StaticData.GetCategories();
            courseViewModel.Institutions = StaticData.GetInstitution();
            return PartialView("_CoursesEditPartial", courseViewModel);
        }
        [HttpGet]
        public IActionResult DetailsPartial(string id)
        {
            var course = _mongoDbService.GetById(ObjectId.Parse(id));
            CourseViewModel courseViewModel = new CourseViewModel();
            courseViewModel.Id = course.Id.ToString();
            courseViewModel.Institution = course.Institution;
            courseViewModel.Duration = course.Duration;
            courseViewModel.Summary = course.Summary;
            courseViewModel.Location = course.Location;
            courseViewModel.Category = course.Category;
            courseViewModel.Name = course.Name;
            courseViewModel.Trainer = course.Trainer;
            courseViewModel.StartDate = course.StartDate;
            return PartialView("_CoursesDetailsPartial", courseViewModel);
        }
        [HttpGet]
        public IActionResult AddCourse()
        {
            CourseViewModel courseViewModel = new CourseViewModel();
            courseViewModel.Locations = StaticData.GetCLocations();
            courseViewModel.Categories = StaticData.GetCategories();
            courseViewModel.Institutions = StaticData.GetInstitution();
            return PartialView("_CoursesAddPartial", courseViewModel);
        }
        [HttpPost]
        public IActionResult DeleteCourse(string id)
        {
            _mongoDbService.DeleteById(ObjectId.Parse(id));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SaveCourse(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var course = new Course
                {

                    Institution = model.Institution,
                    Duration = model.Duration,
                    Summary = model.Summary,
                    Category = model.Category,
                    Location = model.Location,
                    Name = model.Name,
                    StartDate = model.StartDate,
                    Trainer = model.Trainer,
                    Id = ObjectId.GenerateNewId(),
                    LastModified = System.DateTime.Now,
                };
                if (String.IsNullOrEmpty(model.Id))
                    _mongoDbService.Insert(course);
                else
                {
                    course.Id = ObjectId.Parse(model.Id);
                    _mongoDbService.UpdateById(ObjectId.Parse(model.Id), course);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
