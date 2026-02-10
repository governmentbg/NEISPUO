namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IClassBookStudentPrintRepository
{
    Task<GetStudentCoverPageDataVO> GetCoverPageDataAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentTeacherSubjectsVO[]> GetTeacherSubjectsAsync(int schoolYear, int classBookId, int classId, bool classIsLvl2, CancellationToken ct);
    Task<GetStudentSchedulesDataVO[]> GetSchedulesDataAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetStudentSchedulesLessonsVO[]> GetSchedulesLessonsAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetStudentParentMeetingsVO[]> GetParentMeetingsAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<GetStudentExamsVO[]> GetExamsAsync(int schoolYear, int classBookId, BookExamType type, CancellationToken ct);
    Task<GetStudentGradesVO[]> GetGradesAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentRemarksVO[]> GetRemarksAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentFirstGradeResultVO> GetFirstGradeResultAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentAbsencesVO?> GetAbsencesAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentAbsencesByDateVO[]> GetAbsencesByDateAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentGradeResultsVO?> GetGradeResultsAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentGradeResultSessionsVO> GetGradeResultSessionsAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentSanctionsVO[]> GetSanctionsAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentSupportsVO[]> GetSupportsAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentNotesVO[]> GetNotesAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentAttendancesVO[]> GetAttendancesAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentPgResultsVO[]> GetPgResultsAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<GetStudentIndividualWorksVO[]> GetIndividualWorksAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
}
