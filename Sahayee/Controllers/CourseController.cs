using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Driver;
using Sahayee.Models.DB;
using Sahayee.Models.ViewModel;
using Sahayee.Repository;
using System;
using System.Security.Claims;

namespace Sahayee.Controllers
{
    public class CourseController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly MongoDbService<Course> _mongoDbService;
        private readonly MongoDbService<CourseApplication> _mongoDbServiceCA;
        private readonly MongoDbService<News> _mongoDbServiceNews;
        private readonly MongoDbService<Organization> _mongoDbServiceOrganization;

        public CourseController(ILogger<CourseController> logger, MongoDbService<Course> mongoDbService,
            MongoDbService<CourseApplication> mongoDbServiceCA, MongoDbService<News> mongoDbServiceNews, MongoDbService<Organization> mongoDbServiceOrganization)
        {
            _logger = logger;
            _mongoDbService = mongoDbService;
            _mongoDbServiceCA = mongoDbServiceCA;
            _mongoDbServiceNews = mongoDbServiceNews;
            _mongoDbServiceOrganization = mongoDbServiceOrganization;
        }

        [HttpGet]
        public IActionResult Learning()
        {
            var filterCriteria = new Dictionary<string, string>
                {
                    { "Category", "all" },
                    { "CourseType", "all" },
                    { "Institution", "all" },
                };
            var course = _mongoDbService.ApplyFilters(filterCriteria);

            var viewModel = new CourseHomeViewModel
            {
                Courses = course,
                News = _mongoDbServiceNews.Get().Where(x => x.TypeId == "Course").Take(3).ToList(),
                Organization = _mongoDbServiceOrganization.Get().Take(4).ToList(),
                Categories = StaticData.GetCategories()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Courses(string category = "all", string location = "all", string institution = "all", string title = "", int pageNumber = 1, int pageSize = 9)
        {
            List<Course> courses = new List<Course>();
            var filterCriteria = new Dictionary<string, string>
                {
                    { "Category", category },
                    { "CourseType", location },
                    { "Institution", institution },
                };


            if (string.IsNullOrEmpty(title))
                courses = _mongoDbService.ApplyFilters(filterCriteria);
            else
                courses = _mongoDbService.ApplyFilters(title);

            int totalCourses = courses.Count();
            var paginatedCourses = courses
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToList();
            var viewModel = new CourseFilterViewModel
            {
                CategoryId = category,
                CourseTypeId = location,
                InstitutionsId = institution,
                Categories = StaticData.GetCategories(),
                CourseType = StaticData.GetCourseType(),
                Institutions = _mongoDbServiceOrganization.Get().ToList(),
                Courses = paginatedCourses,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCourses / (double)pageSize)
            };

            return View(viewModel);
        }

        [Authorize]
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
                CourseTypes = StaticData.GetCourseType(),
                Institutions = _mongoDbServiceOrganization.Get().ToList(),
                CourseApplicationWithDetails = result
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string id)
        {
            CourseViewModel courseViewModel = new CourseViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                var course = _mongoDbService.GetById(ObjectId.Parse(id));
                courseViewModel = new CourseViewModel
                {
                    Id = course.Id.ToString(),
                    Name = course.Name,
                    Category = course.Category,
                    Institution = course.Institution,
                    Location = course.Location,
                    Duration = course.Duration,
                    Summary = course.Summary,
                    Trainer = course.Trainer,
                    LastModified = DateTime.Now,
                    CourseTitle = course.CourseTitle,
                    CourseType = course.CourseType,

                    // Course Details
                    CourseOverview = course.CourseOverview,
                    LearningObjectives = course.LearningObjectives,
                    CourseContent = course.CourseContent,
                    Prerequisites = course.Prerequisites,

                    // Certification Information
                    CertificationOffered = course.CertificationOffered,
                    CertificateTitle = course.CertificateTitle,
                    CertificationBody = course.CertificationBody,

                    // Enrollment Details
                    StartDate = course.StartDate,
                    EndDate = course.EndDate,
                    ApplicationDeadline = course.ApplicationDeadline,
                    EnrollmentType = course.EnrollmentType,
                    CourseFee = course.CourseFee,
                    PaymentOptions = course.PaymentOptions,

                    // Trainer/Instructor Details
                    InstructorName = course.InstructorName,
                    InstructorBio = course.InstructorBio,
                    InstructorPhotoPath = course.InstructorPhotoPath,

                    // Additional Features
                    TargetAudience = course.TargetAudience,
                    Benefits = course.Benefits,
                    Language = course.Language,
                    Accreditation = course.Accreditation,

                    // Media and Content
                    CourseImagePath = course.CourseImagePath,
                    PromoVideo = course.PromoVideo,

                    // Application Process
                    RequiredDocuments = course.RequiredDocuments,
                    HowToApply = course.HowToApply,

                    // Optional Fields
                    Tags = course.Tags,
                    CourseLevel = course.CourseLevel,
                    Ratings = course.Ratings,
                    ProgressTracking = course.ProgressTracking,
                };
            }
            courseViewModel.CourseTypes = StaticData.GetCourseType();
            courseViewModel.Categories = StaticData.GetCategories();
            courseViewModel.Institutions = _mongoDbServiceOrganization.Get().ToList();
            return View(courseViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CourseViewModel viewModel, IFormFile? instructorPhoto, IFormFile? courseImage)
        {
            // Handle file uploads (if any)
            if (instructorPhoto != null)
            {
                var instructorPhotoPath = Path.Combine(Guid.NewGuid() + instructorPhoto.FileName);
                var logoPath = Path.Combine("wwwroot/uploads", instructorPhotoPath);
                using (var stream = new FileStream(logoPath, FileMode.Create))
                {
                    await instructorPhoto.CopyToAsync(stream);
                }
                viewModel.InstructorPhotoPath = instructorPhotoPath;
            }

            if (courseImage != null)
            {
                var courseImagePath = Path.Combine(Guid.NewGuid() + courseImage.FileName);
                var logoPath = Path.Combine("wwwroot/uploads", courseImagePath);

                using (var stream = new FileStream(logoPath, FileMode.Create))
                {
                    await courseImage.CopyToAsync(stream);
                }
                viewModel.CourseImagePath = courseImagePath;

            }
            var course = new Course
            {
                Id = string.IsNullOrEmpty(viewModel.Id) ? MongoDB.Bson.ObjectId.GenerateNewId() : ObjectId.Parse(viewModel.Id),
                Name = viewModel.Name,
                Category = viewModel.Category,
                Institution = viewModel.Institution,
                Location = viewModel.Location,
                Duration = viewModel.Duration,
                Summary = viewModel.Summary,
                Trainer = viewModel.Trainer,
                LastModified = DateTime.Now,
                CourseTitle = viewModel.CourseTitle,
                CourseType = viewModel.CourseType,

                // Course Details
                CourseOverview = viewModel.CourseOverview,
                LearningObjectives = viewModel.LearningObjectives,
                CourseContent = viewModel.CourseContent,
                Prerequisites = viewModel.Prerequisites,

                // Certification Information
                CertificationOffered = viewModel.CertificationOffered,
                CertificateTitle = viewModel.CertificateTitle,
                CertificationBody = viewModel.CertificationBody,

                // Enrollment Details
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate,
                ApplicationDeadline = viewModel.ApplicationDeadline,
                EnrollmentType = viewModel.EnrollmentType,
                CourseFee = viewModel.CourseFee,
                PaymentOptions = viewModel.PaymentOptions,

                // Trainer/Instructor Details
                InstructorName = viewModel.InstructorName,
                InstructorBio = viewModel.InstructorBio,
                InstructorPhotoPath = viewModel.InstructorPhotoPath,

                // Additional Features
                TargetAudience = viewModel.TargetAudience,
                Benefits = viewModel.Benefits,
                Language = viewModel.Language,
                Accreditation = viewModel.Accreditation,

                // Media and Content
                CourseImagePath = viewModel.CourseImagePath,
                PromoVideo = viewModel.PromoVideo,

                // Application Process
                RequiredDocuments = viewModel.RequiredDocuments,
                HowToApply = viewModel.HowToApply,

                // Optional Fields
                Tags = viewModel.Tags,
                CourseLevel = viewModel.CourseLevel,
                Ratings = viewModel.Ratings,
                ProgressTracking = viewModel.ProgressTracking,
            };
            if (string.IsNullOrEmpty(viewModel.Id))
            {
                // Save the model to MongoDB

                _mongoDbService.Insert(course);
            }
            else
            {
                _mongoDbService.UpdateById(course.Id, course);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DetailsView(string id)
        {
            var result = _mongoDbService.GetById(ObjectId.Parse(id));
            var Institute = _mongoDbServiceOrganization.GetById(ObjectId.Parse(result.Institution));
            result.Institution = Institute.Name;
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

            TempData["Message"] = "You've successfully enrolled in the course!";
            TempData["Success"] = true;
            return RedirectToAction("DetailsView", new { id = id });
        }

        [HttpGet]
        public IActionResult ReloadCourse(string category = "all", string location = "all", string institution = "all")
        {
            var filterCriteria = new Dictionary<string, string>
                {
                    { "Category", category },
                    { "CourseType", location },
                    { "Institution", institution },
                };
            var course = _mongoDbService.ApplyFilters(filterCriteria);
            var viewModel = new CourseFilterViewModel
            {
                Categories = StaticData.GetCategories(),
                CourseType = StaticData.GetCourseType(),
                Institutions = _mongoDbServiceOrganization.Get().ToList(),
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
                    { "CourseType", "all" },
                    { "Institution", "all" },
                };
            var course = _mongoDbService.ApplyFilters(filterCriteria);

            var viewModel = new CourseFilterViewModel
            {
                Categories = StaticData.GetCategories(),
                CourseType = StaticData.GetCourseType(),
                Institutions = _mongoDbServiceOrganization.Get().ToList(),
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
                    { "CourseType", location },
                    { "Institution", institution },
                };
            var course = _mongoDbService.ApplyFilters(filterCriteria);


            return PartialView("_CoursesTablePartial", course);
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

        [HttpPost]
        public IActionResult DeleteCourse(string id)
        {
            _mongoDbService.DeleteById(ObjectId.Parse(id));
            return RedirectToAction("Index");
        }


    }
}
