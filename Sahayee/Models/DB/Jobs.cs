using MongoDB.Bson;

namespace Sahayee.Models.DB
{
    public class Jobs
    {
        public ObjectId Id { get; set; }  // MongoDB _id field        
        public DateTime LastModified { get; set; }

        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty; // Full-time, Part-time, etc.
        public string JobLocation { get; set; } = string.Empty; // City, State, Country
        public bool IsRemote { get; set; } // True if remote or hybrid job
        public string SalaryRange { get; set; } = string.Empty; // Minimum-Maximum salary or Negotiable
        public string SalaryOption { get; set; } = string.Empty; // Option: "Negotiable" or "Confidential"

        // Job Description
        public string JobOverview { get; set; } = string.Empty;
        public List<string> Responsibilities { get; set; } = new List<string>();
        public List<string> RequiredSkills { get; set; } = new List<string>();
        public List<string> CertificationsRequired { get; set; } = new List<string>();

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
        public List<string> DocumentsRequired { get; set; } = new List<string>();

        // Company Details
        public string CompanyOverview { get; set; } = string.Empty;
        public string CompanyLogoPath { get; set; } = string.Empty; // File path for logo
        public string Website { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string ContactPersonEmail { get; set; } = string.Empty;
        public string ContactPersonPhone { get; set; } = string.Empty;

        // Additional Information
        public List<string> BenefitsOffered { get; set; } = new List<string>(); // Health Insurance, etc.
        public bool RelocationAssistance { get; set; } // Available or not
        public string EqualOpportunityStatement { get; set; } = string.Empty;

        // Optional Fields
        public string Tags { get; set; } = string.Empty;
        public string JobCategory { get; set; } = string.Empty; // Nursing, Allied Health, etc.
        public bool ApplicationTrackingEnabled { get; set; } // Enable to track candidate applications


    }
}
