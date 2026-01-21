using MON.Models.StudentModels.Class;
using System;
using System.Collections.Generic;

namespace MON.Models
{
    public class StudentClassModel : StudentClassBaseModel
    {
        public int CommuterTypeId { get; set; }
        public int RepeaterId { get; set; }
        public int? StudentSpecialityId { get; set; }
        public int? StudentProfessionId { get; set; }
        public string StudentEduFormName { get; set; }
        public int? StudentEduFormId { get; set; }
        public int BasicClassId { get; set; }
        public int? ClassNumber { get; set; }
        public int Status { get; set; }
        public bool? HasSupportiveEnvironment { get; set; }
        public string SupportiveEnvironment { get; set; }
        public bool? HasIndividualStudyPlan { get; set; }
        public bool? IsFTACOutsourced { get; set; }
        public bool? IsHourlyOrganization { get; set; }
        public bool? IsNotForSubmissionToNra { get; set; }
        public bool? InternationalProtectionStatus { get; set; }
        public int? AdmissionDocumentId { get; set; }
        public int? CurrentStudentClassId { get; set; }
        public int? OresTypeId { get; set; }
        public ClassGroupEditModel ClassGroup { get; set; }
        /// <summary>
        /// Използва при записване в много паралелки едновременно на ЦПРЛ
        /// </summary>
        public List<int> ClassGroups { get; set; }
        public bool? NotPresentFormClassFilter { get; set; }
        public bool IsNotPresentForm { get; set; }
        public string BasicClassName { get; set; }
        public IEnumerable<int> BuildingRooms { get; set; }
        public IEnumerable<short> BuildingAreas { get; set; }
        public IEnumerable<int> AvailableArchitecture { get; set; }
        public IEnumerable<int> SpecialEquipment { get; set; }

        /// <summary>
        /// Има нужда за запис в група/паралелка без да е наличин документ за записване.
        /// Тогава позицията ще е вземем от EduState и ще я подадем на модела.
        /// </summary>
        public int? InitialEnrollmentPosition { get; set; }
        public StudentClassDualFormCompanyModel[] DualFormCompanies { get; set; }
    }
}
