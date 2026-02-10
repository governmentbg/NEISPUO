using System.Collections.Generic;

namespace MON.Models
{
    public class StudentClassEnrollmentValidationResult : ApiValidationResult
    {
        public int? TargetClassId { get; set; }
        public int? TargetBasicClassId { get; set; }
        public int? TargetClassTypeId { get; set; }
        public int? TargetInstitutionId { get; set; }
        public int TargetPositionId { get; set; }
        public int? CurrentStudentClassId { get; set; }
        public int? TargerClassEduFormId { get; set; }
        public int? TargerClassSpecialityId { get; set; }
        public int? TargetClassKindId { get; set; }
        public bool? TargetClassIsNotPresentForm { get; set; }

        /// <summary>
        /// При валидацията изтегляме от базата данните за проверяваните групи/паралелки.
        /// За да не го правим втори път при създаването или редактирането, което следва валидирането, на StudentClass
        /// данните за групата/паралелката ги запаметяваме в тези обекти.
        /// </summary>
        public List<EnrollmentTargetClass> TargetClasses { get; set; } = new List<EnrollmentTargetClass>();
        public bool TargetClassIsValid { get; set; }
        public bool TargetClassIsNotNpo109 { get; set; }

        public EnrollmentTargetClass ToTargetClass()
        {
            return new EnrollmentTargetClass
            {
                TargetClassId = TargetClassId,
                TargetBasicClassId = TargetBasicClassId,
                TargetClassTypeId = TargetClassTypeId,
                TargetInstitutionId = TargetInstitutionId,
                TargetPositionId = TargetPositionId,
                CurrentStudentClassId  = CurrentStudentClassId,
                TargerClassEduFormId = TargerClassEduFormId,
                TargerClassSpecialityId = TargerClassSpecialityId,
                TargetClassIsNotPresentForm = TargetClassIsNotPresentForm
            };
        }
    }

    /// <summary>
    /// Използва се когато имаме записване в множество групи/паралелки.
    /// </summary>
    public class EnrollmentTargetClass
    {
        public int? TargetClassId { get; set; }
        public int? TargetBasicClassId { get; set; }
        public int? TargetClassTypeId { get; set; }
        public int? TargetInstitutionId { get; set; }
        public int TargetPositionId { get; set; }
        public int? CurrentStudentClassId { get; set; }
        public int? TargerClassEduFormId { get; set; }
        public int? TargerClassSpecialityId { get; set; }
        public bool? TargetClassIsNotPresentForm { get; set; }
        public string TargetClassName { get; set; }
        public bool TargetClassIsValid { get; set; }
        public bool TargetClassIsNotNpo109 { get; set; }
    }
}
