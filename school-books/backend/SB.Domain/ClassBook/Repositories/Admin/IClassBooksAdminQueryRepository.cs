namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IClassBooksAdminQueryRepository
{
    public Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        ClassKind classKind,
        string? bookName,
        int? offset,
        int? limit,
        CancellationToken ct);

    public Task<TableResultVO<GetAllVO>> GetAllForTeacherAsync(
        int schoolYear,
        int instId,
        ClassKind classKind,
        string? bookName,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct);

    public Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    public Task<GetInfoVO> GetInfoAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    public Task<TableResultVO<GetCurriculumVO>> GetCurriculumAsync(
        int schoolYear,
        int classBookId,
        int? offset,
        int? limit,
        CancellationToken ct);

    public Task<TableResultVO<GetStudentsVO>> GetStudentsAsync(
        int schoolYear,
        int classBookId,
        int? offset,
        int? limit,
        CancellationToken ct);

    public Task<GetStudentVO> GetStudentAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct);

    public Task<GetStudentNumbersVO[]> GetStudentNumbersAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    public Task<GetSchoolYearProgramVO> GetSchoolYearProgramAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    public Task<TableResultVO<GetPrintsVO>> GetClassBookPrintsAsync(
        int schoolYear,
        int classBookId,
        int? offset,
        int? limit,
        CancellationToken ct);

    public Task<TableResultVO<GetStudentPrintsVO>> GetClassBookStudentPrintsAsync(
        int schoolYear,
        int classBookId,
        int? offset,
        int? limit,
        CancellationToken ct);

    public Task<bool> HasPendingPrintAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    public Task<bool> HasPendingStudentPrintAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct);

    public Task RemoveRelatedPersonnelSchoolBookAccessAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);
}
