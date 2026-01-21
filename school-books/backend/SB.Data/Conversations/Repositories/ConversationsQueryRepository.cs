namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Domain;
using Microsoft.EntityFrameworkCore;
using static Domain.IConversationParticipantsNomRepository;
using static Domain.IConversationsQueryRepository;

internal class ConversationsQueryRepository : Repository, IConversationsQueryRepository
{
    private const int defaultLimit = 20;

    public ConversationsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetUnreadConversationsVO[]> GetUnreadConversationsAsync(
        int sysUserId,
        int? limit,
        CancellationToken ct)
    {
        return await (
                from c in this.DbContext.Set<Conversation>()

                join cp in this.DbContext.Set<ConversationParticipant>()
                    on new { c.SchoolYear, c.ConversationId } equals new { cp.SchoolYear, cp.ConversationId }

                where ConversationsHelper.SchoolYearsRange().Contains(cp.SchoolYear) &&
                      cp.SysUserId == sysUserId &&
                      (cp.LastReadMessageDate == null || cp.LastReadMessageDate < c.LastMessageDate)

                orderby c.LastMessageDate descending

                select new GetUnreadConversationsVO(
                    c.SchoolYear,
                    c.ConversationId,
                    c.Title,
                    c.LastMessageDate)
            )
            .Take(limit ?? defaultLimit)
            .ToArrayAsync(ct);
    }

    public async Task<GetConversationsVO[]> GetConversationsAsync(
        int sysUserId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var query =
            from c in this.DbContext.Set<Conversation>()

            join cp in this.DbContext.Set<ConversationParticipant>()
                on new { c.SchoolYear, c.ConversationId } equals new { cp.SchoolYear, cp.ConversationId }

            where ConversationsHelper.SchoolYearsRange().Contains(cp.SchoolYear) && cp.SysUserId == sysUserId

            orderby c.LastMessageDate descending

            select new
            {
                c.SchoolYear,
                c.ConversationId,
                c.Title,
                c.LastMessageDate,
                HasNewMessages = cp.LastReadMessageDate == null || cp.LastReadMessageDate < c.LastMessageDate
            };

        return await query
            .Select(x =>
                new GetConversationsVO(
                    x.SchoolYear,
                    x.ConversationId,
                    x.Title,
                    x.HasNewMessages,
                    x.LastMessageDate))
            .WithOffsetAndLimit(offset, limit)
            .ToArrayAsync(ct);
    }

    public async Task<GetConversationInfoVO> GetConversationInfoAsync(
        int schoolYear,
        int conversationId,
        int sysUserId,
        CancellationToken ct)
    {
        return await (
                from c in this.DbContext.Set<Conversation>()

                join cp in this.DbContext.Set<ConversationParticipant>()
                    on new { c.SchoolYear, c.ConversationId } equals new { cp.SchoolYear, cp.ConversationId }

                where c.SchoolYear == schoolYear &&
                      c.ConversationId == conversationId

                select new
                {
                    SchoolYear = c.SchoolYear,
                    ConversationTitle = c.Title,
                    ConversationParticipantsInfo = c.ParticipantsInfo,
                    ConversationIsLocked = c.IsLocked,
                    ConversationCreateDate = c.CreateDate,
                    ParticipantId = cp.ConversationParticipantId,
                    ParticipantTitle = cp.Title,
                    ParticipantIsCreator = cp.IsCreator,
                    ParticipantSysUserId = cp.SysUserId,
                    ParticipantReadLastMessage = cp.LastReadMessageId == c.LastMessageId
                })
            .GroupBy(c =>
                new
                {
                    c.SchoolYear,
                    c.ConversationTitle,
                    c.ConversationParticipantsInfo,
                    c.ConversationIsLocked,
                    c.ConversationCreateDate
                })
            .Select(g =>
                new GetConversationInfoVO(
                    g.Key.SchoolYear,
                    g.Key.ConversationTitle,
                    g.Key.ConversationParticipantsInfo,
                    g.Key.ConversationIsLocked,
                    g.Any(c => c.ParticipantSysUserId == sysUserId && c.ParticipantIsCreator),
                    g.Key.ConversationCreateDate,
                    g.Select(x =>
                            new GetConversationParticipantsVO(
                                x.ParticipantId,
                                x.ParticipantTitle,
                                x.ParticipantIsCreator,
                                x.ParticipantReadLastMessage))
                        .ToArray()))
            .SingleAsync(ct);
    }

    public async Task<GetConversationParticipantsVO[]> GetConversationParticipantsAsync(
        int schoolYear,
        int conversationId,
        int sysUserId,
        CancellationToken ct)
    {
        return await (
            from c in this.DbContext.Set<Conversation>()

            join cp in this.DbContext.Set<ConversationParticipant>()
                on new { c.SchoolYear, c.ConversationId } equals new { cp.SchoolYear, cp.ConversationId }

            where c.SchoolYear == schoolYear &&
                  c.ConversationId == conversationId

            select new GetConversationParticipantsVO(
                cp.ConversationParticipantId,
                cp.Title,
                cp.SysUserId == sysUserId,
                cp.LastReadMessageId == c.LastMessageId)
            )
            .ToArrayAsync(ct);
    }

    public async Task<GetConversationMessagesVO[]> GetConversationMessagesAsync(
        int schoolYear,
        int conversationId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
                from c in this.DbContext.Set<Conversation>()

                join cm in this.DbContext.Set<ConversationMessage>()
                    on new { c.SchoolYear, c.ConversationId } equals new { cm.SchoolYear, cm.ConversationId }

                where c.SchoolYear == schoolYear &&
                      c.ConversationId == conversationId

                orderby cm.CreateDate descending

                select new GetConversationMessagesVO(
                    cm.ConversationMessageId,
                    cm.Content,
                    cm.CreatedByParticipantId,
                    cm.CreateDate)
            )
            .WithOffsetAndLimit(offset, limit ?? defaultLimit)
            .ToArrayAsync(ct);
    }

    public async Task<ConversationParticipantsNomVOParticipant[]> GetTeachersParticipantsAsync(
        int instId,
        int? classBookId,
        CancellationToken ct)
    {
        if (classBookId != null)
        {
            var classBook = await (
                from cb in this.DbContext.Set<ClassBook>()
                where cb.SchoolYear == ConversationsHelper.CurrentSchoolYear() &&
                      cb.ClassBookId == classBookId &&
                      cb.IsValid
                select new
                {
                    cb.ClassId,
                    cb.ClassIsLvl2
                }
            ).SingleAsync(ct);

            var participants = await (
                    from cc in this.DbContext.CurriculumClassForClassBook(ConversationsHelper.CurrentSchoolYear(), classBook.ClassId,
                        classBook.ClassIsLvl2)
                    join t in this.DbContext.Set<CurriculumTeacher>()
                        on cc.CurriculumId equals t.CurriculumId
                    join sp in this.DbContext.Set<StaffPosition>()
                        on t.StaffPositionId equals sp.StaffPositionId
                    join p in this.DbContext.Set<Person>()
                        on sp.PersonId equals p.PersonId
                    join su in this.DbContext.Set<SysUser>().Where(su => su.DeletedOn == null)
                        on sp.PersonId equals su.PersonId
                    where cc.IsValid && t.IsValid && sp.IsValid
                    select new ConversationParticipantsNomVOParticipant(
                        instId,
                        su.SysUserId,
                        StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName) + " (" + ParticipantType.Teacher.GetEnumDescription() + ")",
                        classBookId!.Value,
                        ParticipantType.Teacher))
                .Distinct()
                .ToArrayAsync(ct);

            return participants;
        }

        return await (
                from sp in this.DbContext.Set<StaffPosition>()
                join teacher in this.DbContext.Set<CurriculumTeacher>()
                    on sp.StaffPositionId equals teacher.StaffPositionId
                join p in this.DbContext.Set<Person>()
                    on sp.PersonId equals p.PersonId
                join su in this.DbContext.Set<SysUser>().Where(su => su.DeletedOn == null)
                    on p.PersonId equals su.PersonId

                where sp.InstitutionId == instId && sp.IsValid && teacher.IsValid

                select new ConversationParticipantsNomVOParticipant(
                    instId,
                    su.SysUserId,
                    StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName) + " (" + ParticipantType.Teacher.GetEnumDescription() + ")",
                    null,
                    ParticipantType.Teacher))
                .Distinct()
                .ToArrayAsync(ct);
    }

    public async Task<ConversationParticipantsNomVOParticipant[]> GetParentParticipantsAsync(
        int instId,
        int? classBookId,
        CancellationToken ct)
    {
        var classBookPredicate = PredicateBuilder.True<ClassBook>();
        classBookPredicate = classBookPredicate
            .And(cb => cb.SchoolYear == ConversationsHelper.CurrentSchoolYear() && cb.InstId == instId && cb.IsValid);

        if (classBookId != null)
        {
            classBookPredicate = classBookPredicate.And(cb => cb.ClassBookId == classBookId);
        }

        var parentsCombinedResults =
            (from cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate)
             join sc in this.DbContext.Set<StudentClass>()
                 on new { cb.ClassId, cb.SchoolYear } equals new { sc.ClassId, sc.SchoolYear }
             join pa in this.DbContext.Set<ParentChildSchoolBookAccess>()
                 on sc.PersonId equals pa.ChildId
             where pa.HasAccess
                   && cb.ClassIsLvl2
                   && sc.IsNotPresentForm == false
                   && sc.Status == StudentClassStatus.Enrolled
             select new
             {
                 cb.ClassBookId,
                 pa.ParentId,
                 ChildPersonId = sc.PersonId
             })
            .Concat(
                from cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate)

                join cg in this.DbContext.Set<ClassGroup>()
                    on new { ClassId = (int?)cb.ClassId, cb.SchoolYear } equals new { ClassId = cg.ParentClassId, cg.SchoolYear }
                join sc in this.DbContext.Set<StudentClass>() on new { cg.ClassId, cg.SchoolYear } equals new { sc.ClassId, sc.SchoolYear }
                join pa in this.DbContext.Set<ParentChildSchoolBookAccess>() on sc.PersonId equals pa.ChildId

                where pa.HasAccess
                      && cb.ClassIsLvl2 == false
                      && sc.IsNotPresentForm == false
                      && sc.Status == StudentClassStatus.Enrolled

                select new
                {
                    cb.ClassBookId,
                    pa.ParentId,
                    ChildPersonId = sc.PersonId
                }
            );

        return await (
            from pr in parentsCombinedResults

            join child in this.DbContext.Set<Person>() on pr.ChildPersonId equals child.PersonId
            join parent in this.DbContext.Set<Person>() on pr.ParentId equals parent.PersonId
            join su2 in this.DbContext.Set<SysUser>().Where(su => su.DeletedOn == null) on parent.PersonId equals su2.PersonId

            select new ConversationParticipantsNomVOParticipant(
                instId,
                su2.SysUserId,
                StringUtils.JoinNames(parent.FirstName, parent.MiddleName, parent.LastName) + " (родител на " +
                StringUtils.JoinNames(child.FirstName, child.MiddleName, child.LastName) + ")",
                pr.ClassBookId,
                ParticipantType.Parent)
            )
            .Distinct()
            .ToArrayAsync(ct);
    }

    public async Task<ConversationParticipantsNomVOParticipant> GetCreatorParticipantInfoAsync(
        int instId,
        int sysUserId,
        ParticipantType participant,
        CancellationToken ct)
    {
        return await (
            from su in this.DbContext.Set<SysUser>().Where(su => su.DeletedOn == null)

            join p in this.DbContext.Set<Person>() on su.PersonId equals p.PersonId

            where su.SysUserId == sysUserId

            select new ConversationParticipantsNomVOParticipant(
                instId,
                su.SysUserId,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                null,
                participant))
            .SingleAsync(ct);
    }

    public async Task<ConversationParticipantsNomVOParticipant[]> GetClassLeadersAsync(
        int instId,
        int[] parentSysUserIds,
        int[] teacherSysUserIds,
        CancellationToken ct)
    {
        var schoolYear = ConversationsHelper.CurrentSchoolYear();

        var studentIds = await (
            from psu in this.DbContext.Set<SysUser>().Where(pu => pu.DeletedOn == null)

            join pa in this.DbContext.Set<ParentChildSchoolBookAccess>().Where(pa => pa.HasAccess)
                on psu.PersonId equals pa.ParentId

            where this.DbContext.MakeIdsQuery(parentSysUserIds).Any(ids => psu.SysUserId == ids.Id)

            select pa.ChildId)
        .Distinct()
        .ToArrayAsync(ct);

        var teachersClassBooks = await (
                from tcb in this.DbContext.Set<TeacherCurriculumClassBooksView>()

                join tsu in this.DbContext.Set<SysUser>().Where(tsu => tsu.DeletedOn == null) on tcb.PersonId equals tsu.PersonId
                join cb in this.DbContext.Set<ClassBook>() on new { tcb.SchoolYear, tcb.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

                where
                    tcb.SchoolYear == schoolYear &&
                    tcb.InstId == instId &&
                    this.DbContext.MakeIdsQuery(teacherSysUserIds).Any(ids => tsu.SysUserId == ids.Id)

                select tcb.ClassBookId)
            .Concat(
                from psba in this.DbContext.Set<PersonnelSchoolBookAccess>()

                join tsu in this.DbContext.Set<SysUser>().Where(tsu => tsu.DeletedOn == null) on psba.PersonId equals tsu.PersonId
                join cb in this.DbContext.Set<ClassBook>() on new { psba.SchoolYear, psba.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

                where
                    cb.SchoolYear == schoolYear &&
                    cb.InstId == instId &&
                    this.DbContext.MakeIdsQuery(teacherSysUserIds).Any(ids => tsu.SysUserId == ids.Id) &&
                    cb.IsValid

                select cb.ClassBookId)
            .Distinct()
            .ToArrayAsync(ct);

        var classLeaders = await (
            from cb in this.DbContext.ClassBooksForStudents(schoolYear, studentIds)

            join cg in this.DbContext.Set<ClassGroup>() on cb.ClassId equals cg.ClassId
            join cc in this.DbContext.Set<CurriculumClass>() on cg.ParentClassId equals cc.ClassId
            join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId
            join cut in this.DbContext.Set<CurriculumTeacher>() on c.CurriculumId equals cut.CurriculumId
            join sp in this.DbContext.Set<StaffPosition>() on cut.StaffPositionId equals sp.StaffPositionId
            join p in this.DbContext.Set<Person>() on sp.PersonId equals p.PersonId
            join su in this.DbContext.Set<SysUser>().Where(su => su.DeletedOn == null) on p.PersonId equals su.PersonId

            where this.DbContext.MakeIdsQuery(teachersClassBooks).Any(ids => cb.ClassBookId == ids.Id) &&
                cb.InstId == instId &&
                cc.IsValid &&
                cg.IsValid &&
                cut.IsValid &&
                (c.SubjectId == 199 && cg.BasicClassId > 0) || (c.SubjectId == 80 && cg.BasicClassId < 0)

            select new
            {
                su.SysUserId,
                p.PersonId,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                cb.ClassBookId,
                cb.FullBookName
            })
            .Concat(
                from cb in this.DbContext.ClassBooksForStudents(schoolYear, studentIds)

                join cg in this.DbContext.Set<ClassGroup>() on cb.ClassId equals cg.ClassId
                join cc in this.DbContext.Set<CurriculumClass>() on cg.ClassId equals cc.ClassId
                join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId
                join cut in this.DbContext.Set<CurriculumTeacher>() on c.CurriculumId equals cut.CurriculumId
                join sp in this.DbContext.Set<StaffPosition>() on cut.StaffPositionId equals sp.StaffPositionId
                join p in this.DbContext.Set<Person>() on sp.PersonId equals p.PersonId
                join su in this.DbContext.Set<SysUser>().Where(su => su.DeletedOn == null) on p.PersonId equals su.PersonId

                where this.DbContext.MakeIdsQuery(teachersClassBooks).Any(ids => cb.ClassBookId == ids.Id) &&
                    cb.InstId == instId &&
                    cc.IsValid &&
                    cg.IsValid &&
                    cut.IsValid &&
                    (c.SubjectId == 199 && cg.BasicClassId > 0) || (c.SubjectId == 80 && cg.BasicClassId < 0)

                select new
                {
                    su.SysUserId,
                    p.PersonId,
                    p.FirstName,
                    p.MiddleName,
                    p.LastName,
                    cb.ClassBookId,
                    cb.FullBookName
                })
            .Distinct()
            .ToArrayAsync(ct);

        return classLeaders
            .Select(t =>
                new ConversationParticipantsNomVOParticipant(
                    instId,
                    t.SysUserId,
                    StringUtils.JoinNames(t.FirstName, t.MiddleName, t.LastName) + " (класен ръководител - " + t.FullBookName + ")",
                    t.ClassBookId,
                    ParticipantType.Teacher)
            )
            .ToArray();
    }

    public async Task<bool> CheckIfUserBelongToTheConversation(
        int sysUserId,
        int schoolYear,
        int conversationId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<ConversationParticipant>()
            .AnyAsync(cp =>
                cp.SchoolYear == schoolYear &&
                cp.SysUserId == sysUserId &&
                cp.ConversationId == conversationId,
                ct);
    }
}
