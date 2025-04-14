using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class JobsViewModel
    {
        public string? Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty; // Full-time, Part-time, etc.
        public string JobLocation { get; set; } = string.Empty; // City, State, Country
        public bool IsRemote { get; set; } // True if remote or hybrid job
        public string SalaryRange { get; set; } = string.Empty; // Minimum-Maximum salary or Negotiable
        public string SalaryOption { get; set; } = string.Empty; // Option: "Negotiable" or "Confidential"

        // Job Description
        public string JobOverview { get; set; } = string.Empty;
        public string Responsibilities { get; set; } = string.Empty;
        public string RequiredSkills { get; set; } = string.Empty;
        public string CertificationsRequired { get; set; } = string.Empty;

        // Qualifications
        public string EducationRequirements { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = string.Empty; // Years of experience required/preferred
        public string PreferredQualifications { get; set; } = string.Empty;

        // Work Environment
        public string WorkplaceSetting { get; set; } = string.Empty; // Hospital, Clinic, Remote, Other
        public string ShiftDetails { get; set; } = string.Empty; // Day, Night, Rotational, etc.
        public string WorkHours { get; set; } = string.Empty; // Total hours per week

        // Application Details
        public DateTime? ApplicationDeadline { get; set; }
        public string HowToApply { get; set; } = string.Empty; // Direct Apply, Email, External link
        public string DocumentsRequired { get; set; } = string.Empty;

        // Company Details
        public string CompanyOverview { get; set; } = string.Empty;
        public string CompanyLogoPath { get; set; } = string.Empty; // File path for logo
        public string Website { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string ContactPersonEmail { get; set; } = string.Empty;
        public string ContactPersonPhone { get; set; } = string.Empty;

        // Additional Information
        public string BenefitsOffered { get; set; } = string.Empty;// Health Insurance, etc.
        public bool RelocationAssistance { get; set; } // Available or not
        public string EqualOpportunityStatement { get; set; } = string.Empty;

        // Optional Fields
        public string Tags { get; set; } = string.Empty;
        public string JobCategory { get; set; } = string.Empty; // Nursing, Allied Health, etc.
        public bool ApplicationTrackingEnabled { get; set; } // Enable to track candidate applications

        public List<CommonList>? Locations { get; set; }
        public List<CommonList>? Companies { get; set; }
        public List<JobTypes>? JobTypes { get; set; }
        public List<Categories>? JobCategories { get; set; }
    }

    public class CommonList
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class JobTypes
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
