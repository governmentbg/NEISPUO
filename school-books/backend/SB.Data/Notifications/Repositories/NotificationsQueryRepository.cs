namespace SB.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;

internal class NotificationsQueryRepository : Repository, INotificationsQueryRepository
{
    public NotificationsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<(int? SysUserId, int PersonId, string Name)> GetStudentPersonInfoAsync(int personId, CancellationToken ct)
    {
        var result = await (
            from p in this.DbContext.Set<Person>()
            join su in this.DbContext.Set<SysUser>().Where(u => u.DeletedOn == null) on p.PersonId equals su.PersonId
            into j0
            from su in j0.DefaultIfEmpty()
            where p.PersonId == personId
            select new
            {
                SysUserId = (int?)su.SysUserId,
                p.PersonId,
                Name = StringUtils.JoinNames(p.FirstName, p.LastName)
            }
        ).SingleAsync(ct);

        return (SysUserId: result.SysUserId, PersonId: result.PersonId, Name: result.Name);
    }

    public async Task<string> GetCurriculumNameAsync(int schoolYear, int curriculumId, CancellationToken ct)
    {
        return await (
            from c in this.DbContext.Set<Curriculum>()
            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId
            where c.SchoolYear == schoolYear && c.CurriculumId == curriculumId && c.IsValid
            select $"{s.SubjectName} / {st.Name}"
        ).SingleAsync(ct);
    }

    public async Task<(int SysUserId, int PersonId, string Email)[]> GetStudentRelativeEmailsAsync(int studentPersonId, CancellationToken ct)
    {
        var result = await (
            from pa in this.DbContext.Set<ParentChildSchoolBookAccess>()
            join su in this.DbContext.Set<SysUser>().Where(s => s.DeletedOn == null) on pa.ParentId equals su.PersonId

            where pa.ChildId == studentPersonId && pa.HasAccess

            select new
            {
                su.SysUserId,
                pa.ParentId,
                Email = su.Username
            })
            .ToArrayAsync(ct);

        return result.Select(ri => (SysUserId: ri.SysUserId, PersonId: ri.ParentId, Email: ri.Email)).ToArray();
    }

    public async Task<string> GetCurriculumNameForScheduleLessonAsync(
        int schoolYear,
        int instId,
        int scheduleLessonId,
        CancellationToken ct)
    {
        return await (
            from sl in this.DbContext.Set<ScheduleLesson>()

            join c in this.DbContext.Set<Curriculum>() on sl.CurriculumId equals c.CurriculumId
            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where c.SchoolYear == schoolYear && c.InstitutionId == instId && sl.ScheduleLessonId == scheduleLessonId

            select $"{s.SubjectName} / {st.Name}"
        ).SingleAsync(ct);
    }

    public async Task<(int SysUserId, int PersonId, string Email)[]> GetParticipantsAsync(int conversationId, CancellationToken ct)
    {
        var result = await (
            from c in this.DbContext.Set<ConversationParticipant>()
            join su in this.DbContext.Set<SysUser>().Where(u => u.DeletedOn == null) on c.SysUserId equals su.SysUserId
            where c.ConversationId == conversationId
            select new { su.SysUserId, su.PersonId, Email = su.Username }
        )
        .Distinct()
        .ToArrayAsync(ct);

        return result.Select(x => (x.SysUserId, x.PersonId, x.Email)).ToArray();
    }
}
