using MongoDB.Bson;

namespace Sahayee.Models.DB
{
    public class User
    {
        public ObjectId Id { get; set; }  // MongoDB _id field
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public UserType UserType { get; set; }

        public string Nationality { get; set; }
        public string CurrentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string PassportNumber { get; set; }
        public string Occupation { get; set; }
        public string Specialization { get; set; }
        public int YearsOfExperience { get; set; }
        public string CurrentEmployer { get; set; }
        public string ProfessionalLicenseNumber { get; set; }
        public string[] LanguagesSpoken { get; set; }
        public string PreferredCountryForWork { get; set; }
        public string HighestQualification { get; set; }
        public string InstitutionName { get; set; }
        public string VisaStatus { get; set; }
        public int YearOfGraduation { get; set; }
        public string[] Certifications { get; set; }
        //public User()
        //{
        //    // Set the ObjectId manually (if required)
        //    Id = ObjectId.GenerateNewId();  // Automatically generates a new ObjectId
        //}
    }

    public class UserType
    {
        public int Id { get; set; }
        public string Type { get; set; }

    }
}
