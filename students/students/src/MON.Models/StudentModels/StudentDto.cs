using System;

namespace MON.Models.StudentModels
{
    public class StudentDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public long? PublicEduNumber { get; set; }
        public string PersonalId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string District { get; set; }
        public string Municipality { get; set; }
        public int? InstitutionId { get; set; }
        public int? InstitutionRegionId { get; set; }
        public string School { get; set; }
        public bool? HasRelocationDocuments { get; set; }
        public string Position { get; set; }
    }
}
