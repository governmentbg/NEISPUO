namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface INotificationsQueryRepository
{
    Task<(int? SysUserId, int PersonId, string Name)> GetStudentPersonInfoAsync(int personId, CancellationToken ct);

    Task<string> GetCurriculumNameAsync(int schoolYear, int curriculumId, CancellationToken ct);

    Task<(int SysUserId, int PersonId, string Email)[]> GetStudentRelativeEmailsAsync(int studentPersonId, CancellationToken ct);

    Task<string> GetCurriculumNameForScheduleLessonAsync(
        int schoolYear,
        int instId,
        int scheduleLessonId,
        CancellationToken ct);

    Task<(int SysUserId, int PersonId, string Email)[]> GetParticipantsAsync(int conversationId, CancellationToken ct);
}
