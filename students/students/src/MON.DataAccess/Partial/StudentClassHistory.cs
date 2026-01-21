using MON.Shared.Interfaces;

namespace MON.DataAccess
{
    public partial class StudentClassHistory : ICreatable, IUpdatable
    {
        internal static StudentClassHistory FromStudentClassEntity(StudentClass e)
        {
            return new StudentClassHistory
            {
                StudentClassId = e.Id,
                SchoolYear = e.SchoolYear,
                ClassId = e.ClassId,
                PersonId = e.PersonId,
                StudentSpecialityId = e.StudentSpecialityId,
                StudentEduFormId = e.StudentEduFormId,
                ClassNumber = e.ClassNumber,
                Status = e.Status,
                IsIndividualCurriculum = e.IsIndividualCurriculum,
                IsHourlyOrganization = e.IsHourlyOrganization,
                IsForSubmissionToNra = e.IsForSubmissionToNra,
                IsFtacoutsourced = e.IsFtacoutsourced,
                IsCurrent = e.IsCurrent,
                RepeaterId = e.RepeaterId,
                CommuterTypeId = e.CommuterTypeId,
                HasSuportiveEnvironment = e.HasSuportiveEnvironment,
                SupportiveEnvironment = e.SupportiveEnvironment,
                EnrollmentDate = e.EnrollmentDate,
                AdmissionDocumentId = e.AdmissionDocumentId,
                PositionId = e.PositionId,
                BasicClassId = e.BasicClassId,
                ClassTypeId = e.ClassTypeId,
                FromStudentClassId = e.FromStudentClassId,
                DischargeReasonId = e.DischargeReasonId,
                RelocationDocumentId = e.RelocationDocumentId,
                DischargeDocumentId = e.DischargeDocumentId,
                OrestypeId = e.OrestypeId,
                InstitutionId = e.InstitutionId
            };
        }
    }
}