namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IClassBookPrintRepository
{
    Task<GetCoverPageDataVO> GetCoverPageDataAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetTeacherSubjectsVO[]> GetTeacherInfoAndSubjectsAsync(int schoolYear, int classBookId, int classId, bool classIsLvl2, CancellationToken ct);
    Task<GetSchedulesDataVO[]> GetSchedulesDataAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetSchedulesLessonsVO[]> GetSchedulesLessonsAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetParentMeetingsVO[]> GetParentMeetingsAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetExamsVO[]> GetExamsAsync(int schoolYear, int classBookId, BookExamType type, CancellationToken ct);
    Task<GetStudentsDataVO[]> GetStudentsDataAsync(int schoolYear, int instId, int classBookId, int classId, bool classIsLvl2, CancellationToken ct);
    Task<GetStudentsDataVO[]> GetRemovedStudentsDataAsync(int schoolYear, int classBookId, int[] studentPersonIds, CancellationToken ct);
    Task<GetGradesVO[]> GetGradesAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetRemarksVO[]> GetRemarksAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetScheduleAndAbsencesByWeekVO[]> GetScheduleAndAbsencesByWeekAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetFirstGradeResultsVO[]> GetFirstGradeResultsAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetAbsencesVO[]> GetAbsencesAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetAbsencesByDateVO[]> GetAbsencesByDateAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetGradeResultsVO[]> GetGradeResultsAsync(int schoolYear, int classBookId, int classId, bool classIsLvl2, CancellationToken ct);
    Task<GetGradeResultSessionsVO[]> GetGradeResultSessionsAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetSanctionsVO[]> GetSanctionsAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetSupportsVO[]> GetSupportsAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetNotesVO[]> GetNotesAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetAttendancesVO[]> GetAttendancesAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetPgResultsVO[]> GetPgResultsAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetIndividualWorksVO[]> GetIndividualWorksAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetClassBookInfoVO> GetClassBookInfoAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetCurriculumInfoVO[]> GetCurriculumAsync(int schoolYear, int classBookId, int classId, bool classIsLvl2, CancellationToken ct);
    Task<GetReplrParticipationsVO[]> GetReplrParticipationsAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetPerformancesVO[]> GetPerformancesAsync(int schoolYear, int classBookId, CancellationToken ct);
}
