using MongoDB.Bson;
using Sahayee.Models.DB;

namespace Sahayee.Models.ViewModel
{
    public class CourseViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Institution { get; set; }
        public string Location { get; set; }
        public string Duration { get; set; }
        public string Summary { get; set; }
        public string Trainer { get; set; }
        public DateTime LastModified { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;

        // Course Details
        public string CourseOverview { get; set; } = string.Empty;
        public string LearningObjectives { get; set; } = string.Empty;
        public string CourseContent { get; set; } = string.Empty;
        public string? Prerequisites { get; set; }

        // Certification Information
        public bool CertificationOffered { get; set; }
        public string? CertificateTitle { get; set; }
        public string? CertificationBody { get; set; }

        // Enrollment Details
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public string EnrollmentType { get; set; } = string.Empty;
        public decimal? CourseFee { get; set; }
        public string PaymentOptions { get; set; } = string.Empty;

        // Trainer/Instructor Details
        public string InstructorName { get; set; } = string.Empty;
        public string InstructorBio { get; set; } = string.Empty;
        public string? InstructorPhotoPath { get; set; }

        // Additional Features
        public string TargetAudience { get; set; } = string.Empty;
        public string? Benefits { get; set; }
        public string Language { get; set; } = string.Empty;
        public string? Accreditation { get; set; }

        // Media and Content
        public string? CourseImagePath { get; set; }
        public string? PromoVideo { get; set; }

        // Application Process
        public string RequiredDocuments { get; set; } = string.Empty;
        public string HowToApply { get; set; } = string.Empty;

        // Optional Fields
        public string? Tags { get; set; }
        public string? CourseLevel { get; set; }
        public double? Ratings { get; set; }
        public bool? ProgressTracking { get; set; }

        public List<Categories>? Categories { get; set; }
        public List<Organization>? Institutions { get; set; }
        public List<CLocations>? CourseTypes { get; set; }
    }
}
