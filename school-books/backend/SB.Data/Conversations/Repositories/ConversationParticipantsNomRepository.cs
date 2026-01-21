namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Domain;
using Microsoft.EntityFrameworkCore;
using static Domain.IConversationParticipantsNomRepository;

internal class ConversationParticipantsNomRepository : Repository, IConversationParticipantsNomRepository
{
    private readonly ParticipantType[] singlePersonParticipant =
    {
        ParticipantType.Teacher,
        ParticipantType.Parent
    };

    public ConversationParticipantsNomRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<ConversationParticipantsNomVO[]> GetNomsByIdAsync(
        int instId,
        ConversationParticipantsNomVOParticipant[] ids,
        CancellationToken ct)
    {
        var participantPredicate = PredicateBuilder.True<ConversationParticipant>();
        participantPredicate = participantPredicate
            .And(p =>
                p.SchoolYear == ConversationsHelper.CurrentSchoolYear() &&
                p.InstId == instId);

        var sysUserIds = ids.Select(id => id.SysUserId ?? 0).ToArray();

        var participants = await (
                from cp in this.DbContext.Set<ConversationParticipant>().Where(participantPredicate)
                join cpg in this.DbContext.Set<ConversationParticipantGroup>() on
                    new { cp.SchoolYear, GroupId = cp.ConversationParticipantGroupId } equals
                    new { cpg.SchoolYear, GroupId = (int?)cpg.ConversationParticipantGroupId }
                    into j1
                from cpg in j1.DefaultIfEmpty()
                where this.DbContext.MakeIdsQuery(sysUserIds).Any(id => cp.SysUserId == id.Id)
                select new
                {
                    cp.InstId,
                    cp.SysUserId,
                    cp.Title,
                    cp.ParticipantType,
                    GroupName = (string?)cpg.GroupName,
                    cpg.ClassBookId,
                    GroupType = (int?)cpg.ParticipantType
                })
            .ToArrayAsync(ct);

        var groups = participants
            .Where(p => p.GroupName != null && p.GroupType != null)
            .GroupBy(p => new { p.GroupType, p.GroupName, p.ClassBookId })
            .Select(g => new ConversationParticipantsNomVO(
                new ConversationParticipantsNomVOParticipant(
                    instId,
                    null,
                    g.Key.GroupName,
                    g.Key.ClassBookId,
                    (ParticipantType)g.Key.GroupType!),
                g.Key.GroupName))
            .ToArray();

        var singlePersons = participants
            .Where(p => this.singlePersonParticipant.Contains(p.ParticipantType))
            .Select(p => new ConversationParticipantsNomVO(
                new ConversationParticipantsNomVOParticipant(
                    instId,
                    p.SysUserId,
                    p.Title,
                    null,
                    p.ParticipantType),
                p.Title))
            .ToArray();

        return groups.Union(singlePersons)
            .OrderBy(d => d.Id.ParticipantType)
            .ThenBy(d => d.Id.Title)
            .ToArray();
    }

    public async Task<ConversationParticipantsNomVO[]> GetNomsByTermAsync(
        int instId,
        SysRole sysRoleId,
        int? personId,
        int[]? studentPersonIds,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return sysRoleId switch
        {
            SysRole.Parent when studentPersonIds != null =>
                await this.GetParticipantsForParentsAsync(instId, studentPersonIds, term, offset, limit, ct),
            SysRole.Teacher when personId != null =>
                await this.GetParticipantsForTeacherAsync(instId, personId.Value, term, offset, limit, ct),
            SysRole.Institution or SysRole.InstitutionExpert =>
                await this.GetParticipantsForAdministratorAsync(instId, term, offset, limit, ct),
            _ => Array.Empty<ConversationParticipantsNomVO>()
        };
    }

    private async Task<ConversationParticipantsNomVO[]> GetParticipantsForParentsAsync(
        int instId,
        int[] studentPersonIds,
        string? term,
        int? offset,
        int? limit,
        CancellationToken cToken)
    {
        var schoolYear = ConversationsHelper.CurrentSchoolYear();

        var personPredicate = PredicateBuilder.True<Person>();

        if (!string.IsNullOrWhiteSpace(term))
        {
            var words = term.Split(null);
            foreach (var word in words)
                personPredicate =
                    personPredicate.AndAnyStringContains(p => p.FirstName, p => p.MiddleName, p => p.LastName, word);
        }

        var classBooks = (
                from cb in this.DbContext.Set<ClassBook>()
                join cg in this.DbContext.Set<ClassGroup>() on new { cb.SchoolYear, ClassId = (int?)cb.ClassId } equals
                    new { cg.SchoolYear, ClassId = cg.ParentClassId }
                join sc in this.DbContext.Set<StudentClass>() on new { cg.SchoolYear, cg.ClassId } equals new
                { sc.SchoolYear, sc.ClassId }
                where sc.IsNotPresentForm == false &&
                      sc.SchoolYear == schoolYear &&
                      sc.InstitutionId == instId &&
                      cb.IsValid &&
                      cb.ClassIsLvl2 == false &&
                      this.DbContext.MakeIdsQuery(studentPersonIds).Any(ids => sc.PersonId == ids.Id) &&
                      sc.Status == StudentClassStatus.Enrolled
                select new
                {
                    cb.InstId,
                    cb.ClassId,
                    cb.ClassIsLvl2,
                    cb.ClassBookId,
                    cb.FullBookName,
                    cb.BasicClassId
                })
            .Concat(
                from cb in this.DbContext.Set<ClassBook>()
                join sc in this.DbContext.Set<StudentClass>()
                    on new { cb.SchoolYear, cb.ClassId } equals new { sc.SchoolYear, sc.ClassId }
                where sc.IsNotPresentForm == false &&
                      sc.SchoolYear == schoolYear &&
                      sc.InstitutionId == instId &&
                      cb.IsValid &&
                      cb.ClassIsLvl2 == true &&
                      this.DbContext.MakeIdsQuery(studentPersonIds).Any(ids => sc.PersonId == ids.Id) &&
                      sc.Status == StudentClassStatus.Enrolled
                select new
                {
                    cb.InstId,
                    cb.ClassId,
                    cb.ClassIsLvl2,
                    cb.ClassBookId,
                    cb.FullBookName,
                    cb.BasicClassId
                }
            );

        var combinedData = (
                from cb in classBooks
                join cg in this.DbContext.Set<ClassGroup>() on cb.ClassId equals cg.ClassId
                join cc in this.DbContext.Set<CurriculumClass>() on cg.ClassId equals cc.ClassId
                join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId
                join ct in this.DbContext.Set<CurriculumTeacher>() on c.CurriculumId equals ct.CurriculumId
                join sp in this.DbContext.Set<StaffPosition>() on ct.StaffPositionId equals sp.StaffPositionId
                join p in this.DbContext.Set<Person>().Where(personPredicate) on sp.PersonId equals p.PersonId
                join su in this.DbContext.Set<SysUser>().Where(su => su.DeletedOn == null) on p.PersonId equals su
                    .PersonId
                where cc.IsValid &&
                      cg.IsValid &&
                      ct.IsValid
                select new
                {
                    su.SysUserId,
                    p.FirstName,
                    p.MiddleName,
                    p.LastName,
                    cb.BasicClassId,
                    cb.ClassBookId,
                    cb.FullBookName,
                    IsClassLeader = (c.SubjectId == 199 && cg.BasicClassId > 0) || (c.SubjectId == 80 && cg.BasicClassId < 0)
                })
            .Concat(
                from cb in classBooks
                join cg in this.DbContext.Set<ClassGroup>() on cb.ClassId equals cg.ClassId
                join cc in this.DbContext.Set<CurriculumClass>() on cg.ParentClassId equals cc.ClassId
                join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId
                join ct in this.DbContext.Set<CurriculumTeacher>() on c.CurriculumId equals ct.CurriculumId
                join sp in this.DbContext.Set<StaffPosition>() on ct.StaffPositionId equals sp.StaffPositionId
                join p in this.DbContext.Set<Person>().Where(personPredicate) on sp.PersonId equals p.PersonId
                join su in this.DbContext.Set<SysUser>().Where(su => su.DeletedOn == null) on p.PersonId equals su
                    .PersonId
                where cc.IsValid &&
                      cg.IsValid &&
                      ct.IsValid
                select new
                {
                    su.SysUserId,
                    p.FirstName,
                    p.MiddleName,
                    p.LastName,
                    cb.BasicClassId,
                    cb.ClassBookId,
                    cb.FullBookName,
                    IsClassLeader = (c.SubjectId == 199 && cg.BasicClassId > 0) || (c.SubjectId == 80 && cg.BasicClassId < 0)
                }
            );

        var result = await combinedData
            .Distinct()
            .GroupBy(x => new { x.SysUserId, x.FirstName, x.MiddleName, x.LastName })
            .OrderBy(t => t.Key.FirstName)
            .ThenBy(t => t.Key.MiddleName)
            .ThenBy(t => t.Key.LastName)
            .Select(group => group.OrderByDescending(x => x.IsClassLeader).First())
            .WithOffsetAndLimit(offset, limit)
            .ToArrayAsync(cToken);

        return result
            .Select(t =>
            {
                var typeToString = t.IsClassLeader ? $"класен ръководител - {t.FullBookName}" : "учител";
                var title = $"{StringUtils.JoinNames(t.FirstName, t.MiddleName, t.LastName)} ({typeToString})";

                return new ConversationParticipantsNomVO(
                    new ConversationParticipantsNomVOParticipant(instId, t.SysUserId, title, t.ClassBookId, ParticipantType.Teacher),
                    title
                );
            })
            .ToArray();
    }

    private async Task<ConversationParticipantsNomVO[]> GetParticipantsForTeacherAsync(
        int instId,
        int personId,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var schoolYear = ConversationsHelper.CurrentSchoolYear();

        var classBooks = (
                from tcb in this.DbContext.Set<TeacherCurriculumClassBooksView>()
                join cb in this.DbContext.Set<ClassBook>() on new { tcb.SchoolYear, tcb.ClassBookId } equals new
                { cb.SchoolYear, cb.ClassBookId }
                where
                    tcb.PersonId == personId &&
                    tcb.SchoolYear == schoolYear &&
                    tcb.InstId == instId &&
                    cb.IsValid
                select new
                {
                    tcb.SchoolYear,
                    tcb.ClassId,
                    cb.BasicClassId,
                    tcb.ClassBookId,
                    cb.FullBookName
                })
            .Concat(
                from psba in this.DbContext.Set<PersonnelSchoolBookAccess>()
                join cb in this.DbContext.Set<ClassBook>()
                    on new { psba.SchoolYear, psba.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
                where
                    psba.PersonId == personId &&
                    cb.SchoolYear == schoolYear &&
                    cb.InstId == instId &&
                    cb.IsValid
                select new
                {
                    cb.SchoolYear,
                    cb.ClassId,
                    cb.BasicClassId,
                    cb.ClassBookId,
                    cb.FullBookName
                })
            .Distinct();

        var query = (
                from cb in classBooks

                select new
                {
                    SysUserId = (int?)null,
                    Title = "Родители (" + cb.FullBookName + ")",
                    cb.BasicClassId,
                    ClassbookId = (int?)cb.ClassBookId,
                    Type = ParticipantType.ParentsForClass
                })
            .Concat(
                from cb in classBooks

                join cg in this.DbContext.Set<ClassGroup>() on new { cb.SchoolYear, ClassId = (int?)cb.ClassId } equals
                    new { cg.SchoolYear, ClassId = cg.ParentClassId }
                join sc in this.DbContext.Set<StudentClass>() on cg.ClassId equals sc.ClassId
                join child in this.DbContext.Set<Person>() on sc.PersonId equals child.PersonId
                join pa in this.DbContext.Set<ParentChildSchoolBookAccess>() on child.PersonId equals pa.ChildId
                join psu in this.DbContext.Set<SysUser>().Where(pu => pu.DeletedOn == null) on pa.ParentId equals psu
                    .PersonId
                join parent in this.DbContext.Set<Person>() on psu.PersonId equals parent.PersonId

                where sc.IsNotPresentForm == false &&
                      sc.Status == StudentClassStatus.Enrolled &&
                      cg.IsValid

                select new
                {
                    SysUserId = (int?)psu.SysUserId,
                    Title = StringUtils.JoinNames(parent.FirstName, parent.MiddleName, parent.LastName) + " (родител на " + StringUtils.JoinNames(child.FirstName, child.MiddleName, child.LastName) + ")",
                    BasicClassId = (int?)null,
                    ClassbookId = (int?)null,
                    Type = ParticipantType.Parent
                })
            .Concat(
                from cb in classBooks

                join cg in this.DbContext.Set<ClassGroup>() on cb.ClassId equals cg.ClassId
                join sc in this.DbContext.Set<StudentClass>() on cg.ClassId equals sc.ClassId
                join child in this.DbContext.Set<Person>() on sc.PersonId equals child.PersonId
                join pa in this.DbContext.Set<ParentChildSchoolBookAccess>() on child.PersonId equals pa.ChildId
                join psu in this.DbContext.Set<SysUser>().Where(pu => pu.DeletedOn == null) on pa.ParentId equals psu
                    .PersonId
                join parent in this.DbContext.Set<Person>() on psu.PersonId equals parent.PersonId

                where sc.IsNotPresentForm == false &&
                      sc.Status == StudentClassStatus.Enrolled &&
                      cg.IsValid

                select new
                {
                    SysUserId = (int?)psu.SysUserId,
                    Title = StringUtils.JoinNames(parent.FirstName, parent.MiddleName, parent.LastName) + " (родител на " + StringUtils.JoinNames(child.FirstName, child.MiddleName, child.LastName) + ")",
                    BasicClassId = (int?)null,
                    ClassbookId = (int?)null,
                    Type = ParticipantType.Parent
                }
            );

        var predicate = PredicateBuilder.True(query);
        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(p => p.Title, word);
            }
        }

        return await
            query
            .Distinct()
            .Where(predicate)
            .OrderBy(p => p.Type)
            .ThenBy(p => p.BasicClassId)
            .ThenBy(p => p.Title)
            .WithOffsetAndLimit(offset, limit)
            .Select(p => new ConversationParticipantsNomVO(
                new ConversationParticipantsNomVOParticipant(
                    instId,
                    p.SysUserId,
                    p.Title,
                    p.ClassbookId,
                    p.Type),
                p.Title))
            .ToArrayAsync(ct);
    }

    private async Task<ConversationParticipantsNomVO[]> GetParticipantsForAdministratorAsync(
        int instId,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var schoolYear = ConversationsHelper.CurrentSchoolYear();

        var classBooks = (
                from tcb in this.DbContext.Set<TeacherCurriculumClassBooksView>()
                join cb in this.DbContext.Set<ClassBook>() on new { tcb.SchoolYear, tcb.ClassBookId } equals new
                { cb.SchoolYear, cb.ClassBookId }
                where
                    tcb.SchoolYear == schoolYear &&
                    tcb.InstId == instId &&
                    cb.IsValid
                select new
                {
                    tcb.SchoolYear,
                    tcb.ClassId,
                    cb.BasicClassId,
                    tcb.ClassBookId,
                    cb.FullBookName
                })
            .Concat(
                from psba in this.DbContext.Set<PersonnelSchoolBookAccess>()
                join cb in this.DbContext.Set<ClassBook>()
                    on new { psba.SchoolYear, psba.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
                where
                    cb.SchoolYear == schoolYear &&
                    cb.InstId == instId &&
                    cb.IsValid
                select new
                {
                    cb.SchoolYear,
                    cb.ClassId,
                    cb.BasicClassId,
                    cb.ClassBookId,
                    cb.FullBookName
                })
            .Distinct();

        var participantsByClassBook = (
                from cb in classBooks

                select new
                {
                    SysUserId = (int?)null,
                    Title = "Родители (" + cb.FullBookName + ")",
                    BasicClassId = cb.BasicClassId,
                    ClassbookId = (int?)cb.ClassBookId,
                    Type = ParticipantType.ParentsForClass
                })
            .Concat(
                from cb in classBooks

                select new
                {
                    SysUserId = (int?)null,
                    Title = "Учители (" + cb.FullBookName + ")",
                    BasicClassId = cb.BasicClassId,
                    ClassbookId = (int?)cb.ClassBookId,
                    Type = ParticipantType.TeachersForClass
                }
            );

        var parentParticipants = (
                from cb in classBooks

                join cg in this.DbContext.Set<ClassGroup>() on new { cb.SchoolYear, ClassId = (int?)cb.ClassId } equals
                    new { cg.SchoolYear, ClassId = cg.ParentClassId }
                join sc in this.DbContext.Set<StudentClass>() on cg.ClassId equals sc.ClassId
                join child in this.DbContext.Set<Person>() on sc.PersonId equals child.PersonId
                join pa in this.DbContext.Set<ParentChildSchoolBookAccess>() on child.PersonId equals pa.ChildId
                join psu in this.DbContext.Set<SysUser>().Where(pu => pu.DeletedOn == null) on pa.ParentId equals psu
                    .PersonId
                join parent in this.DbContext.Set<Person>() on psu.PersonId equals parent.PersonId

                where sc.IsNotPresentForm == false &&
                      sc.Status == StudentClassStatus.Enrolled &&
                      cg.IsValid

                select new
                {
                    SysUserId = (int?)psu.SysUserId,
                    Title = StringUtils.JoinNames(parent.FirstName, parent.MiddleName, parent.LastName) +
                            " (родител на " + StringUtils.JoinNames(child.FirstName, child.MiddleName, child.LastName) + ")",
                    BasicClassId = (int?)null,
                    ClassbookId = (int?)null,
                    Type = ParticipantType.Parent
                })
            .Concat(
                from cb in classBooks

                join cg in this.DbContext.Set<ClassGroup>() on cb.ClassId equals cg.ClassId
                join sc in this.DbContext.Set<StudentClass>() on cg.ClassId equals sc.ClassId
                join child in this.DbContext.Set<Person>() on sc.PersonId equals child.PersonId
                join pa in this.DbContext.Set<ParentChildSchoolBookAccess>() on child.PersonId equals pa.ChildId
                join psu in this.DbContext.Set<SysUser>().Where(pu => pu.DeletedOn == null) on pa.ParentId equals psu
                    .PersonId
                join parent in this.DbContext.Set<Person>() on psu.PersonId equals parent.PersonId

                where sc.IsNotPresentForm == false &&
                      sc.Status == StudentClassStatus.Enrolled &&
                      cg.IsValid

                select new
                {
                    SysUserId = (int?)psu.SysUserId,
                    Title = StringUtils.JoinNames(parent.FirstName, parent.MiddleName, parent.LastName) +
                            " (родител на " + StringUtils.JoinNames(child.FirstName, child.MiddleName, child.LastName) + ")",
                    BasicClassId = (int?)null,
                    ClassbookId = (int?)null,
                    Type = ParticipantType.Parent
                }
            );

        var teacherParticipants =
            from sp in this.DbContext.Set<StaffPosition>()

            join teacher in this.DbContext.Set<CurriculumTeacher>() on sp.StaffPositionId equals teacher.StaffPositionId
            join p in this.DbContext.Set<Person>() on sp.PersonId equals p.PersonId
            join su in this.DbContext.Set<SysUser>().Where(su => su.DeletedOn == null) on p.PersonId equals su.PersonId

            where sp.InstitutionId == instId && sp.IsValid && teacher.IsValid

            select new
            {
                SysUserId = (int?)su.SysUserId,
                Title = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName) + " (учител)",
                BasicClassId = (int?)null,
                ClassbookId = (int?)null,
                Type = ParticipantType.Teacher
            };

        var query = participantsByClassBook
            .Concat(parentParticipants)
            .Concat(teacherParticipants);

        var predicate = PredicateBuilder.True(query);
        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(p => p.Title, word);
            }
        }

        return await
            query
            .Where(predicate)
            .Distinct()
            .OrderBy(p => p.Type)
            .ThenBy(p => p.BasicClassId)
            .ThenBy(p => p.Title)
            .WithOffsetAndLimit(offset, limit)
            .Select(p => new ConversationParticipantsNomVO(
                new ConversationParticipantsNomVOParticipant(
                    instId,
                    p.SysUserId,
                    p.Title,
                    p.ClassbookId,
                    p.Type),
                p.Title))
            .ToArrayAsync(ct);
    }
}
