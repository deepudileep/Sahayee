using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Driver;
using Sahayee.Helper;
using Sahayee.Models;
using Sahayee.Models.DB;
using Sahayee.Models.ViewModel;
using Sahayee.Repository;
using System.Diagnostics;
using System.Security.Claims;

namespace Sahayee.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly MongoDbService<User> _mongoDbService;
        private readonly MongoDbService<Jobs> _mongoDbServiceJobs;
        private readonly MongoDbService<News> _mongoDbServiceNews;
        public UserController(ILogger<UserController> logger, MongoDbService<User> mongoDbService,
            MongoDbService<Jobs> mongoDbServiceJobs, MongoDbService<News> mongoDbServiceNews)
        {
            _logger = logger;
            _mongoDbService = mongoDbService;
            _mongoDbServiceJobs = mongoDbServiceJobs;
            _mongoDbServiceNews = mongoDbServiceNews;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [Authorize]
        public IActionResult Profile()
        {
            var userId = string.Empty;
            ProfileViewModel profileViewModel = new ProfileViewModel();
            //var userId = HttpContext.Session.GetString("UserId");
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            var registration = _mongoDbService.GetById(ObjectId.Parse(userId));
            return View(registration);
        }


        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Build filter criteria for MongoDB query
            var filterCriteria = new Dictionary<string, string>
    {
        { "Email", username },
        { "Password", password } // Ensure passwords are securely hashed in production
    };

            // Query user from the database
            var user = _mongoDbService.ApplyFilters(filterCriteria).FirstOrDefault();

            if (user != null)
            {
                // Create user claims
                var claims = new List<Claim>
        {
            //new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Name, user.FirstName+" "+user.LastName),
            new Claim(ClaimTypes.Role, user.UserType.Type), // Assuming 'UserType.Type' represents the role
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

                // Create identity and principal
                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                // Sign in the user with cookies
                await HttpContext.SignInAsync("CookieAuth", principal);
                if (user.UserType.Type == "Admin")
                    return RedirectToAction("AdminDashboard", "Home");

                // Redirect to profile page or desired action
                return RedirectToAction("Index", "Home");
            }

            // Handle invalid login attempt
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Registration()
        {
            RegistrationViewModel model = new RegistrationViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateProfile([FromForm] User updatedUser)
        {
            var existingUser = _mongoDbService.GetById(updatedUser.Id);
            existingUser.FirstName = updatedUser.FirstName ?? existingUser.FirstName;
            existingUser.LastName = updatedUser.LastName ?? existingUser.LastName;
            existingUser.Gender = updatedUser.Gender ?? existingUser.Gender;
            existingUser.DOB = updatedUser.DOB != DateTime.MinValue ? updatedUser.DOB : existingUser.DOB;
            existingUser.Nationality = updatedUser.Nationality ?? existingUser.Nationality;
            existingUser.Email = updatedUser.Email ?? existingUser.Email;
            existingUser.PhoneNumber = updatedUser.PhoneNumber ?? existingUser.PhoneNumber;
            existingUser.Location = updatedUser.Location ?? existingUser.Location;
            existingUser.CurrentAddress = updatedUser.CurrentAddress ?? existingUser.CurrentAddress;
            existingUser.PermanentAddress = updatedUser.PermanentAddress ?? existingUser.PermanentAddress;
            existingUser.PassportNumber = updatedUser.PassportNumber ?? existingUser.PassportNumber;
            existingUser.Occupation = updatedUser.Occupation ?? existingUser.Occupation;
            existingUser.Specialization = updatedUser.Specialization ?? existingUser.Specialization;
            existingUser.YearsOfExperience = updatedUser.YearsOfExperience != 0 ? updatedUser.YearsOfExperience : existingUser.YearsOfExperience;
            existingUser.CurrentEmployer = updatedUser.CurrentEmployer ?? existingUser.CurrentEmployer;
            existingUser.ProfessionalLicenseNumber = updatedUser.ProfessionalLicenseNumber ?? existingUser.ProfessionalLicenseNumber;
            existingUser.LanguagesSpoken = updatedUser.LanguagesSpoken ?? existingUser.LanguagesSpoken;
            existingUser.PreferredCountryForWork = updatedUser.PreferredCountryForWork ?? existingUser.PreferredCountryForWork;
            existingUser.HighestQualification = updatedUser.HighestQualification ?? existingUser.HighestQualification;
            existingUser.InstitutionName = updatedUser.InstitutionName ?? existingUser.InstitutionName;
            existingUser.VisaStatus = updatedUser.VisaStatus ?? existingUser.VisaStatus;
            existingUser.YearOfGraduation = updatedUser.YearOfGraduation != 0 ? updatedUser.YearOfGraduation : existingUser.YearOfGraduation;
            existingUser.Certifications = updatedUser.Certifications ?? existingUser.Certifications;


            // Save the registration to MongoDB
            _mongoDbService.UpdateById(updatedUser.Id, existingUser);

            // Redirect after successful submission
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult CheckEmailExist(string email)
        {
            var user = _mongoDbService.ApplyFilters(new Dictionary<string, string> { { "Email", email } }).FirstOrDefault();
            return Json(new { exists = user != null });
        }

        [HttpPost]
        public IActionResult CheckEmailPassExist(string email,string pass)
        {
            var user = _mongoDbService.ApplyFilters(new Dictionary<string, string> { { "Email", email }, { "Password", pass } }).FirstOrDefault();
            return Json(new { exists = user != null });
        }

        [HttpPost]
        public async Task<IActionResult> Registration(string FirstName, string LastName, string Email, string PhoneNumber, string password, string cPassword)
        {


            // Create a new registration object to save
            var registration = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = CommonHelper.GenerateRandomPassword(5),
                Location = string.Empty,
                PhoneNumber = PhoneNumber,
                DOB = null,
                Gender = string.Empty,
                UserType = new UserType
                {
                    Id = 1,
                    Type = "User"
                }
            };
            _mongoDbService.Insert(registration);
            var filterCriteria = new Dictionary<string, string>
    {
        { "Email", registration.Email },
        { "Password", registration.Password } // Ensure passwords are securely hashed in production
    };
            // Save the registration to MongoDB
            var user = _mongoDbService.ApplyFilters(filterCriteria).FirstOrDefault();

            // Create user claims
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.UserType.Type), // Assuming 'UserType.Type' represents the role
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

            // Create identity and principal
            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            // Sign in the user with cookies
            await HttpContext.SignInAsync("CookieAuth", principal);

            // Redirect to profile page or desired action


            // Redirect after successful submission
            return RedirectToAction("Index","Home");
        }


        [Authorize]
        public async Task<IActionResult> DashBoard()
        {
            var userId = string.Empty;
            ProfileViewModel profileViewModel = new ProfileViewModel();
            //var userId = HttpContext.Session.GetString("UserId");
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            var registration = _mongoDbService.GetById(ObjectId.Parse(userId));
            profileViewModel.News = _mongoDbServiceNews.Get().ToList();
            if (registration == null)
            {
                return NotFound();
            }
            profileViewModel.User = registration;
            JobCounts jobCounts = new JobCounts();
            jobCounts.Category = await _mongoDbServiceJobs.CountDistinctValuesAsync("JobType");
            jobCounts.Hospital = await _mongoDbServiceJobs.CountDistinctValuesAsync("CompanyName");
            jobCounts.Country = await _mongoDbServiceJobs.CountDistinctValuesAsync("JobLocation");
            profileViewModel.JobCounts = jobCounts;

            return View(profileViewModel);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}