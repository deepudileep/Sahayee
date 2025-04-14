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
                Companies = new List<CommonList>() { new CommonList { Id = "all", Name = "All Companies" } },
                Locations = new List<CommonList>() { new CommonList { Id = "all", Name = "All Locations" } },
                JobTypes = StaticData.GetJobType(),
                Jobs = jobs
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Create(string id)
        {
            JobsViewModel jobsViewModel = new JobsViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                var job = _mongoDbService.GetById(ObjectId.Parse(id));
                jobsViewModel = new JobsViewModel
                {
                    Id = job.Id.ToString(),
                    JobTitle = job.JobTitle,
                    CompanyName = job.CompanyName,
                    JobType = job.JobType,
                    JobLocation = job.JobLocation,
                    IsRemote = job.IsRemote,
                    SalaryRange = job.SalaryRange,
                    JobOverview = job.JobOverview,
                    Responsibilities = string.Join(Environment.NewLine, job.Responsibilities),
                    RequiredSkills = string.Join(Environment.NewLine, job.RequiredSkills),
                    CertificationsRequired = string.Join(Environment.NewLine, job.CertificationsRequired),
                    EducationRequirements = job.EducationRequirements,
                    ExperienceLevel = job.ExperienceLevel,
                    PreferredQualifications = job.PreferredQualifications,
                    WorkplaceSetting = job.WorkplaceSetting,
                    ShiftDetails = job.ShiftDetails,
                    WorkHours = job.WorkHours,
                    ApplicationDeadline = job.ApplicationDeadline,
                    HowToApply = job.HowToApply,
                    DocumentsRequired = string.Join(Environment.NewLine, job.DocumentsRequired),
                    CompanyOverview = job.CompanyOverview,
                    CompanyLogoPath = job.CompanyLogoPath,
                    Website = job.Website,
                    ContactPerson = job.ContactPerson,
                    ContactPersonEmail = job.ContactPersonEmail,
                    ContactPersonPhone = job.ContactPersonPhone,
                    BenefitsOffered = string.Join(Environment.NewLine, job.BenefitsOffered),
                    RelocationAssistance = job.RelocationAssistance,
                    EqualOpportunityStatement = job.EqualOpportunityStatement,
                    Tags = job.Tags,
                    JobCategory = job.JobCategory,
                    ApplicationTrackingEnabled = job.ApplicationTrackingEnabled
                };
            }
            jobsViewModel.Companies = new List<CommonList>() { new CommonList { Id = "all", Name = "All Companies" } };
            jobsViewModel.Locations = new List<CommonList>() { new CommonList { Id = "all", Name = "All Locations" } };
            jobsViewModel.JobTypes = StaticData.GetJobType();
            jobsViewModel.JobCategories = StaticData.GetCategories();
            return View(jobsViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> DetailsView(string id)
        {
            var result = _mongoDbService.GetById(ObjectId.Parse(id));
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(JobsViewModel model, IFormFile? companyLogo)
        {
            // Handle file uploads (if any)
            if (companyLogo != null)
            {
                string filename = Guid.NewGuid() + companyLogo.FileName;
                var companyLogoPath = Path.Combine("wwwroot/uploads", filename);
                using (var stream = new FileStream(companyLogoPath, FileMode.Create))
                {
                    await companyLogo.CopyToAsync(stream);
                }
                model.CompanyLogoPath = "/uploads/" + filename;
            }

            var modelData = new Jobs
            {
                Id = string.IsNullOrEmpty(model.Id) ? MongoDB.Bson.ObjectId.GenerateNewId() : ObjectId.Parse(model.Id),
                JobTitle = model.JobTitle,
                CompanyName = model.CompanyName,
                JobType = model.JobType,
                JobLocation = model.JobLocation,
                IsRemote = model.IsRemote,
                SalaryRange = model.SalaryRange,
                JobOverview = model.JobOverview,
                Responsibilities = model.Responsibilities.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                RequiredSkills = model.RequiredSkills.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                CertificationsRequired = model.CertificationsRequired.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                EducationRequirements = model.EducationRequirements,
                ExperienceLevel = model.ExperienceLevel,
                PreferredQualifications = model.PreferredQualifications,
                WorkplaceSetting = model.WorkplaceSetting,
                ShiftDetails = model.ShiftDetails,
                WorkHours = model.WorkHours,
                ApplicationDeadline = model.ApplicationDeadline, // Default to current date if null
                HowToApply = model.HowToApply,
                DocumentsRequired = model.DocumentsRequired.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                CompanyOverview = model.CompanyOverview,
                CompanyLogoPath = model.CompanyLogoPath,
                Website = model.Website,
                ContactPerson = model.ContactPerson,
                ContactPersonEmail = model.ContactPersonEmail,
                ContactPersonPhone = model.ContactPersonPhone,
                BenefitsOffered = model.BenefitsOffered.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                RelocationAssistance = model.RelocationAssistance,
                EqualOpportunityStatement = model.EqualOpportunityStatement,
                Tags = model.Tags,
                JobCategory = model.JobCategory,
                ApplicationTrackingEnabled = model.ApplicationTrackingEnabled
            };
            if (string.IsNullOrEmpty(model.Id))
            {
                // Save the model to MongoDB

                _mongoDbService.Insert(modelData);
            }
            else
            {
                _mongoDbService.UpdateById(modelData.Id, modelData);
            }
            return RedirectToAction("Index");
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
                Companies = new List<CommonList>() { new CommonList { Id = "all", Name = "All Companies" } },
                Locations = new List<CommonList>() { new CommonList { Id = "all", Name = "All Locations" } },
                JobTypes = StaticData.GetJobType(),
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
                Companies = new List<CommonList>() { new CommonList { Id = "all", Name = "All Companies" } },
                Locations = new List<CommonList>() { new CommonList { Id = "all", Name = "All Locations" } },
                JobTypes = StaticData.GetJobType(),
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


        public IActionResult ReloadJobs(string JobType = "all", string Company = "all", string location = "all")
        {
            var filterCriteria = new Dictionary<string, string>();

            if (JobType != "all") filterCriteria.Add("JobType", JobType);
            if (Company != "all") filterCriteria.Add("CompanyName", Company);
            if (location != "all") filterCriteria.Add("JobLocation", location);

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
        public IActionResult FilterJobs(string JobType = "all", string Company = "all", string location = "all")
        {
            var filterCriteria = new Dictionary<string, string>();

            if (JobType != "all") filterCriteria.Add("JobType", JobType);
            if (Company != "all") filterCriteria.Add("CompanyName", Company);
            if (location != "all") filterCriteria.Add("JobLocation", location);
            var jobs = _mongoDbService.ApplyFilters(filterCriteria);

            return PartialView("_JobTablePartial", jobs);
        }
        [HttpGet]

        [HttpGet]
        public IActionResult AddJob()
        {
            JobsViewModel jobsViewModel = new JobsViewModel();
            jobsViewModel.Companies = new List<CommonList>() { new CommonList { Id = "all", Name = "All Companies" } };
            jobsViewModel.Locations = new List<CommonList>() { new CommonList { Id = "all", Name = "All Locations" } };
            jobsViewModel.JobTypes = StaticData.GetJobType();
            jobsViewModel.JobCategories = StaticData.GetCategories();
            return PartialView("_JobEditPartial", jobsViewModel);
        }
        [HttpPost]
        public IActionResult DeleteJob(string id)
        {
            _mongoDbService.DeleteById(ObjectId.Parse(id));
            return RedirectToAction("Index");
        }


    }
}
