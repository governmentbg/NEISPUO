using System;
using System.Collections.Generic;

namespace MON.Models.Diploma
{
    public class DiplomaViewModel
    {
        public int Id { get; set; }
        public short SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public short? YearGraduated { get; set; }
        public int? TemplateId { get; set; }
        public int? EduFormId { get; set; }
        public decimal? EduDuration { get; set; }
        public string Gpatext { get; set; }
        public decimal? Gpa { get; set; }
        public string ProtocolNumber { get; set; }
        public DateTime? ProtocolDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string Series { get; set; }
        public string FactoryNumber { get; set; }
        public string RegistrationNumberTotal { get; set; }
        public string RegistrationNumberYear { get; set; }
        public string Contents { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PersonalId { get; set; }
        public int PersonalIdType { get; set; }
        public int? NationalityId { get; set; }
        public int? InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string BirthPlaceRegion { get; set; }
        public string BirthPlaceMunicipality { get; set; }
        public string BirthPlaceTown { get; set; }
        public int PersonId { get; set; }
        public int DiplomaId { get; set; }
        public string ReportFormPath { get; set; }
        public bool IsPublic { get; set; }
        public bool IsSigned { get; set; }
        public bool IsMigrated { get; set; }
        public bool IsFinalized { get; set; }
        public string BasicDocumentType { get; set; }
        public bool IsCancelled { get; set; }

        public string FullName { get; set; }
        public int BasicDocumentId { get; set; }
        public string PersonalIdTypeName { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string UpdaterUsername { get; set; }
        public DateTime? SignedDate { get; set; }
        public string SignerUsername { get; set; }
        public bool IsEditable { get; set; }
        public DateTime? EditableSetDate { get; set; }
        public string EditableSetReason { get; set; }
        public string EditableSetUsername { get; set; }

        public bool CanBeSigned { get; set; }
        public bool CanBeDeleted { get; set; }
        public bool CanBeEdit { get; set; }
        public bool CanBeSetAsEditable { get; set; }
        public bool IsValidation { get; set; }

        public List<TagModel> Tags { get; set; } = new List<TagModel>();
        public DateTime? FinalizedDate { get; set; }
        public bool CreatedByMonHrRole { get; set; }
        public int? RegionId { get; set; }
        public bool IsIncludedInRegister { get; set; }
        public bool IsRuoDocBasicDocument { get; set; }

        public int? RuoRegId { get; set; }

        public bool IsNotSignable => !IsIncludedInRegister;

        public string BasicDocumentName { get; set; }

        public IEnumerable<DiplomaAdditionalDocumentViewModel> AdditionalDocuments { get; set; } = Array.Empty<DiplomaAdditionalDocumentViewModel>();
        public string RuoRegCode { get; set; }
        public string RuoRegName { get; set; }
    }
}
