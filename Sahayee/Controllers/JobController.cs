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
    public class JobController : Controller
    {
        private readonly ILogger<JobController> _logger;
        private readonly MongoDbService<Jobs> _mongoDbService;
        private readonly MongoDbService<JobApplication> _mongoDbServiceJobApplication;
        private readonly MongoDbService<FollowUp> _mongoDbServiceFollowUp;
        private readonly MongoDbService<User> _mongoDbServiceUser;

        public JobController(ILogger<JobController> logger, MongoDbService<Jobs> mongoDbService,
            MongoDbService<JobApplication> mongoDbServiceJobApplication,
            MongoDbService<FollowUp> mongoDbServiceFollowUp,
            MongoDbService<User> mongoDbServiceUser)
        {
            _logger = logger;
            _mongoDbService = mongoDbService;
            _mongoDbServiceJobApplication = mongoDbServiceJobApplication;
            _mongoDbServiceFollowUp = mongoDbServiceFollowUp;
            _mongoDbServiceUser = mongoDbServiceUser;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var filterCriteria = new Dictionary<string, string>
                {
                    { "Department", "all" },
                    { "Location", "all" },
                    { "Institution", "all" },
                };
            var jobs = _mongoDbService.ApplyFilters(filterCriteria);

            var viewModel = new JobFilterViewModel
            {
                Position = StaticData.GetPositions(),
                Location = StaticData.GetLocations(),
                Institutions = StaticData.GetInstitution(),
                Jobs = jobs
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Jobs(string position = "all", string location = "all", string institution = "all")
        {
            var filterCriteria = new Dictionary<string, string>
                {
                    { "Department", position },
                    { "Location", location },
                    { "Institution", institution },
                };
            var jobs = _mongoDbService.ApplyFilters(filterCriteria);

            var viewModel = new JobFilterViewModel
            {
                Position = StaticData.GetPositions(),
                Location = StaticData.GetLocations(),
                Institutions = StaticData.GetInstitution(),
                Jobs = jobs
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> MyJobs()
        {
            var userId = string.Empty;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            var result = await _mongoDbService.GetJobsAppliedByUserAsync(userId);
            var viewModel = new JobApplicationDetailsViewModel
            {
                Position = StaticData.GetPositions(),
                Location = StaticData.GetLocations(),
                Institutions = StaticData.GetInstitution(),
                JobApplicationWithDetails = result
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> SaveFollowUp([FromBody] FollowUpUpdateModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.ApplicationId))
            {
                TempData["Message"] = "Application ID is required.";
                return BadRequest(new { success = false, message = "Application ID is required." });
            }

            var inputModel = new FollowUp()
            {
                Notes = model.AdminNotes,
                Status = model.Status,
                Date = DateTime.Now,
                DoneBy = "Admin" // Replace with the actual user's name or ID
            };
            var filter = Builders<JobApplication>.Filter.Eq(j => j.LastModified, inputModel.Date);
            _mongoDbServiceJobApplication.UpdateLastModified(filter);

            try
            {
                var result = await _mongoDbService.UpdateJobFollowUpHistory(model.ApplicationId, inputModel);

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
        public IActionResult ReloadJobs(string position = "all", string location = "all", string institution = "all")
        {
            var filterCriteria = new Dictionary<string, string>();

            if (position != "all") filterCriteria.Add("Department", position);
            if (location != "all") filterCriteria.Add("Location", location);
            if (institution != "all") filterCriteria.Add("Institution", institution);

            var jobs = _mongoDbService.ApplyFilters(filterCriteria);

            // Return a partial view with the filtered jobs
            var viewModel = new JobFilterViewModel
            {
                Jobs = jobs
            };

            return PartialView("_JobListPartial", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ApplyForJob([FromForm] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Message"] = "Job ID is required.";
                return RedirectToAction("Jobs"); // Redirect to the list view
            }
            var userId = string.Empty;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "Please login to apply for this position!";
                return RedirectToAction("Jobs"); // Redirect to the list view
            }

            var filterCriteria = new Dictionary<string, string>
    {
        { "UserId", userId },
        { "JobId", id }
    };

            var jobs = _mongoDbServiceJobApplication.ApplyFilters(filterCriteria);
            if (jobs.Count > 0)
            {
                TempData["Message"] = "Already applied for this position!";
                return RedirectToAction("Jobs");
            }
            var Jdate = DateTime.Now;
            var application = new JobApplication
            {
                Id = ObjectId.GenerateNewId(),
                JobId = id,
                UserId = userId,
                Status = "Applied",
                UserMessage = "",
                AppliedOn = Jdate,
                FollowUpBy = "",
                FollowUpDate = Jdate,
                LastModified = Jdate,
                FollowUpHistory = new List<FollowUp>
                {
                    new FollowUp
                    {
                        Date = Jdate,
                        Status = "Job Applied",
                        Notes = "Initial review completed, sent to HR for evaluation",
                        DoneBy = "Admin"
                    },
                }
            };

            _mongoDbServiceJobApplication.Insert(application);

            TempData["Message"] = "Application submitted successfully!";
            return RedirectToAction("Jobs");
        }

        public async Task<IActionResult> AdminApplicantDetails(string jobAId)
        {
            // Fetch job applications based on jobId if necessary
            var jobApplication = _mongoDbService.GetJobsAppliedByJobAsync(jobAId).Result;
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }
        public async Task<IActionResult> GetFollowUpHistory(string applicantId)
        {
            var followUpHistory = _mongoDbServiceFollowUp.GetById(ObjectId.Parse(applicantId));

            if (followUpHistory == null)
            {
                return PartialView("_FollowUpDetailsPartial", new List<FollowUp>());
            }

            return PartialView("_FollowUpDetailsPartial", followUpHistory);
        }


        [HttpGet]
        public IActionResult FilterJobs(string position = "all", string location = "all", string institution = "all")
        {
            var filterCriteria = new Dictionary<string, string>
                {
                    { "Department", position },
                    { "Location", location },
                    { "Institution", institution },
                };
            var jobs = _mongoDbService.ApplyFilters(filterCriteria);

            return PartialView("_JobTablePartial", jobs);
        }
        [HttpGet]
        public IActionResult EditJob(string id)
        {
            var job = _mongoDbService.GetById(ObjectId.Parse(id));
            JobsViewModel jobsViewModel = new JobsViewModel();
            jobsViewModel.Id = job.Id.ToString();
            jobsViewModel.Institution = job.Institution;
            jobsViewModel.ContactEmail = job.ContactEmail;
            jobsViewModel.Department = job.Department;
            jobsViewModel.Location = job.Location;
            jobsViewModel.JobTitle = job.JobTitle;
            jobsViewModel.Description = job.Description;
            jobsViewModel.Locations = StaticData.GetLocations();
            jobsViewModel.Positions = StaticData.GetPositions();
            jobsViewModel.Institutions = StaticData.GetInstitution();
            return PartialView("_JobEditPartial", jobsViewModel);
        }
        [HttpGet]
        public IActionResult DetailsPartial(string id)
        {
            var job = _mongoDbService.GetById(ObjectId.Parse(id));
            JobsViewModel jobsViewModel = new JobsViewModel();
            jobsViewModel.Id = job.Id.ToString();
            jobsViewModel.Institution = job.Institution;
            jobsViewModel.ContactEmail = job.ContactEmail;
            jobsViewModel.Department = job.Department;
            jobsViewModel.Location = job.Location;
            jobsViewModel.JobTitle = job.JobTitle;
            jobsViewModel.Description = job.Description;
            return PartialView("_JobDetailsPartial", jobsViewModel);
        }
        [HttpGet]
        public IActionResult AddJob()
        {
            JobsViewModel jobsViewModel = new JobsViewModel();
            jobsViewModel.Locations = StaticData.GetLocations();
            jobsViewModel.Positions = StaticData.GetPositions();
            jobsViewModel.Institutions = StaticData.GetInstitution();
            return PartialView("_JobEditPartial", jobsViewModel);
        }
        [HttpPost]
        public IActionResult DeleteJob(string id)
        {
            _mongoDbService.DeleteById(ObjectId.Parse(id));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SaveJob(JobsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var job = new Jobs
                {
                    ContactEmail = model.ContactEmail,
                    Department = model.Department,
                    Description = model.Description,
                    Institution = model.Institution,
                    JobTitle = model.JobTitle,
                    Location = model.Location,
                    LastModified = DateTime.Now,
                    Id = ObjectId.GenerateNewId()
                };
                if (String.IsNullOrEmpty(model.Id))
                    _mongoDbService.Insert(job);
                else
                {
                    job.Id = ObjectId.Parse(model.Id);
                    job.LastModified = DateTime.Now;
                    _mongoDbService.UpdateById(ObjectId.Parse(model.Id), job);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
