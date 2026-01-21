using System;

namespace MON.Models.Diploma
{
    public class DiplomaModel
    {
        public int DiplomaId { get; set; }
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public short? YearGraduated { get; set; }
        public int? TemplateId { get; set; }
        public int? EduFormId { get; set; }
        public decimal? EduDuration { get; set; }
        public decimal? Gpa { get; set; }
        public string Gpatext { get; set; }
        public string StateExamQualificationGradeText { get; set; }
        public decimal? StateExamQualificationGrade { get; set; }
        public string ProtocolNumber { get; set; }
        public DateTime? ProtocolDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Series { get; set; }
        public string FactoryNumber { get; set; }
        public string RegistrationNumberTotal { get; set; }
        public string RegistrationNumberYear { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string Contents { get; set; }
        public string FirstName { get; set; }
        public string FirstNameLatin { get; set; }
        public string MiddleName { get; set; }
        public string MiddleNameLatin { get; set; }
        public string LastName { get; set; }
        public string LastNameLatin { get; set; }
        public string PersonalId { get; set; }
        public short PersonalIdType { get; set; }
        public string BirthPlaceTown { get; set; }
        public string BirthPlaceMunicipality { get; set; }
        public string BirthPlaceRegion { get; set; }
        public string BirthPlaceLocalArea { get; set; }
        public int? GenderId { get; set; }
        public bool IsDiplomaFormPrinted { get; set; }
        public int? SPPOOProfessionId { get; set; }
        public int? SPPOOSpecialityId { get; set; }
        public string LeadTeacher { get; set; }
        public string Principal { get; set; }
        public string Director { get; set; }
        public string Deputy { get; set; }
        public string ProfessionPart { get; set; }
        public short? VetLevel { get; set; }
        public string Qualification { get; set; }
        public string FLLevel { get; set; }
        public string FLGELevel { get; set; }
        public int? ITLevel { get; set; }
        public int? BasicClass { get; set; }
        public int? NKR { get; set; }
        public int? EKR { get; set; }
        public string Session { get; set; }
        public string Description { get; set; }
        public DropdownViewModel EduForm { get; set; }
        public DropdownViewModel EduType { get; set; }
        public DropdownViewModel Nationality { get; set; }
        public DropdownViewModel Institution { get; set; }
        public DropdownViewModel Gender { get; set; }
        public DropdownViewModel SPPOOProfession { get; set; }
        public string SPPOOProfessionName { get; set; }
        public DropdownViewModel SPPOOSpeciality { get; set; }
        public string SPPOOSpecialityName { get; set; }
        public DropdownViewModel Profile { get; set; }
        public DropdownViewModel ClassType { get; set; }
        public string ClassTypeName { get; set; }
        public DropdownViewModel Ministry { get; set; }
    }
}
