namespace MON.Models.Refugee
{
    using MON.Shared.Enums;
    using System;

    public class ApplicationChildModel
    {
        public int? Id { get; set; }
        public int? PersonId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        public DateTime BirthDate { get; set; }
        public int NationalityId { get; set; }
        public int Gender { get; set; }
        public int ProtectionStatus { get; set; }
        public int? TownId { get; set; }
        public string Address { get; set; }
        public int LastInstitutionCountry { get; set; }
        public string RuoDocNumber { get; set; }
        public DateTime? RuoDocDate { get; set; }
        public int BgLanguageSkill { get; set; }
        public int EnLanguageSkill { get; set; }
        public int DeLanguageSkill { get; set; }
        public int FrLanguageSkill { get; set; }
        public string OtherLanguage { get; set; }
        public int? OtherLanguageSkill { get; set; }
        public int LastInstitutionType { get; set; }
        public int? LastBasicClassId { get; set; }
        public bool? IsClassCompleted { get; set; }
        public bool HasNeedForTextbooks { get; set; }
        public bool HasNeedForResourceSupport { get; set; }
        public bool IsForCsop { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PersonalId { get; set; }
        public int PersonalIdType { get; set; }
        public DropdownViewModel PersonalIdTypeModel { get; set; }
        public string Profession { get; set; }
        public short? SchoolYear { get; set; }
        public int? InstitutionId { get; set; }
        public bool HasDualCitizenship { get; set; }
        public bool HasDocumentForCompletedClass { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }

        public bool HasValidRouOrderAttrs => !string.IsNullOrWhiteSpace(RuoDocNumber)
           && RuoDocDate.HasValue && InstitutionId.HasValue;

        public bool CanBeDeleted => Status != (int)ApplicationStatusEnum.Completed;
        public bool CanBeCancelled => Status != (int)ApplicationStatusEnum.Cancelled;
        public bool CanBeEdited => Status == (int)ApplicationStatusEnum.InProcess;
        public bool CanBeCompleted => Status == (int)ApplicationStatusEnum.InProcess
            && HasValidRouOrderAttrs;

        public bool CanBeSetAsEditable => Status != (int)ApplicationStatusEnum.InProcess;
    }
}
