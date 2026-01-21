namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IExtApiQueryRepository
{
    Task<AbsenceDO[]> AbsencesGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<AttendanceDO[]> AttendancesGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<ClassBookDO[]> ClassBooksGetAllAsync(
        int schoolYear,
        int institutionId,
        CancellationToken ct);

    Task<ExamDO[]> ExamsGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<FirstGradeResultDO[]> FirstGradeResultsGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<GradeDO[]> GradesGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<GradeResultDO[]> GradeResultsGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<NoteDO[]> NotesGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<OffDayDO[]> OffDaysGetAllAsync(
        int schoolYear,
        int institutionId,
        CancellationToken ct);

    Task<ParentMeetingDO[]> ParentMeetingsGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<PgResultDO[]> PgResultsGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<RemarkDO[]> RemarksGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<TopicDO[]> TopicsGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<SanctionDO[]> SanctionsGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<ShiftDO[]> ShiftsGetAllAsync(
        int schoolYear,
        int institutionId,
        CancellationToken ct);

    Task<ScheduleDO[]> SchedulesGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<SchoolYearDateInfoDO[]> SchoolYearSettingsGetAllAsync(
        int schoolYear,
        int institutionId,
        CancellationToken ct);

    Task<SupportDO[]> SupportsGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<IndividualWorkDO[]> IndividualWorksGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<PerformanceDO[]> PerformancesGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<ReplrParticipationDO[]> ReplrParticipationsGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<TeacherAbsenceDO[]> TeacherAbsencesGetAllAsync(
        int schoolYear,
        int institutionId,
        CancellationToken ct);

    Task<AdditionalActivityDO[]> AdditionalActivitiesGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<StudentActivitiesDO[]> StudentActivitiesGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<StudentClassNumberDO[]> StudentClassNumbersGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<StudentGradelessCurriculumsDO[]> StudentGradelessCurriculumsGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<StudentSpecialNeedCurriculumsDO[]> StudentSpecialNeedCurriculumsGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<StudentCarriedAbsencesDO[]> StudentCarriedAbsencesGetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);
}
