namespace SB.Api;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IAuthService
{
    Task<bool> HasInstitutionAccessAsync(
        OidcToken token,
        AccessType accessType,
        int instId,
        CancellationToken ct);

    Task<bool> HasInstitutionAdminAccessAsync(
        OidcToken token,
        AccessType accessType,
        int instId,
        CancellationToken ct);

    Task<bool> HasClassBookAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct);

    Task<bool> HasStudentInfoClassBookAccessAsync(
        OidcToken token,
        int schoolYear,
        int instId,
        int classBookId,
        int personId,
        CancellationToken ct);

    Task<bool> HasClassBookAdminAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct);

    Task<bool> HasCurriculumAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        int curriculumId,
        CancellationToken ct);

    Task<bool> HasReplCurriculumAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        int curriculumId,
        DateTime date,
        CancellationToken ct);

    Task<bool> HasScheduleLessonAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleLessonId,
        CancellationToken ct);

    Task<bool> HasSupportAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        int supportId,
        CancellationToken ct);

    Task<bool> HasAttendanceDateAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        DateTime date,
        CancellationToken ct);

    Task<bool> HasStudentAccessAsync(
        OidcToken token,
        CancellationToken ct);

    Task<bool> HasStudentClassBookAccessAsync(
        OidcToken token,
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct);

    Task<bool> HasStudentMedicalNoticesAccessAsync(
        OidcToken token,
        int schoolYear,
        int personId,
        CancellationToken ct);

    Task<bool> HasHisMedicalNoticeAccessAsync(
        OidcToken token,
        int hisMedicalNoticeId,
        CancellationToken ct);

    Task<bool> HasConversationsAccessAsync(
        OidcToken token,
        AccessType accessType,
        int instId,
        CancellationToken ct);
}
