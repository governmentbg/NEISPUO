using System;

namespace MON.Models.StudentModels
{
    public class StudentClassViewModel : StudentClassModel
    {
        public string StudentSpeciality { get; set; }
        public string StudentProfession { get; set; }
        public string StudentEduForm { get; set; }
        public string RepeaterReason { get; set; }
        public string CommuterTypeName { get; set; }
        public new ClassGroupViewModel ClassGroup { get; set; }
        public new DateTime EnrollmentDate { get; set; }
        public int PositionId { get; set; }
        public string Position { get; set; }
        public string SchoolYearName { get; set; }
        public bool IsCurrent { get; set; }
        public string StatusName { get; set; }
        public bool HasHistory { get; set; }
        public string CreatedBySysUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifiedBySysUser { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string OresTypeName { get; set; }
        public new bool? IsNotPresentForm { get; set; }
        public bool IsBasicClassForCurrentInstitution { get; set; }
        public bool IsAdditionalClass { get; set; }
        public bool CanAddCurriculum { get; set; }
        public int InstitutionId { get; set; }
        public bool CanBeDeleted { get; set; }
        public bool HasExternalSoProvider { get; set; }
        public bool HasExternalSoProviderClassTypeLimitation { get; set; }
        public bool HasExternalSoProviderActionLimitation { get; set; }
        public string BasicClassRomeName { get; set; }
        public string ClassGroupName { get; set; }
        public string InstitutionAbbreviation { get; set; }
        public string SpecialEquipmentsStr { get; set; }
        public string AvailableArchitecturesStr { get; set; }
        public string BuildingAreasStr { get; set; }
        public string BuildingRoomsStr { get; set; }
        public bool? IsLodFinalized { get; set; }
        public bool CanChangePosition {  get; set; }
        public int? ChangeTargetPositionId { get; set; }
    }
}
