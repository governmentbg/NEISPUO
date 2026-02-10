namespace SB.Data;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IClassGroupsQueryRepository;

internal class ClassGroupsQueryRepository : Repository, IClassGroupsQueryRepository
{
    public ClassGroupsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllForClassKindVO[]> GetAllForClassKindAsync(
        int schoolYear,
        int instId,
        ClassKind classKind,
        CancellationToken ct)
    {
        var instType = await (
            from i in this.DbContext.Set<InstitutionSchoolYear>()

            join d in this.DbContext.Set<DetailedSchoolType>() on i.DetailedSchoolTypeId equals d.DetailedSchoolTypeId

            where i.SchoolYear == schoolYear &&
                i.InstitutionId == instId

            select d.InstType
        ).SingleAsync(ct);

        var classGroups = await (
            from cg in this.DbContext.Set<ClassGroup>()

            join ctype in this.DbContext.Set<ClassType>() on cg.ClassTypeId equals ctype.ClassTypeId
            into j1 from ctype in j1.DefaultIfEmpty()

            join bc in this.DbContext.Set<BasicClass>() on cg.BasicClassId equals bc.BasicClassId
            into j2 from bc in j2.DefaultIfEmpty()

            join cb in this.DbContext.Set<ClassBook>().Where(cb => cb.IsValid) on new { cg.SchoolYear, cg.ClassId } equals new { cb.SchoolYear, cb.ClassId }
            into j3 from cb in j3.DefaultIfEmpty()

            where cg.SchoolYear == schoolYear &&
                cg.InstitutionId == instId &&
                cg.IsValid &&
                ((ClassKind?)ctype.ClassKind ?? ClassKind.Class) == classKind &&

                // exclude not present form (IFO) system class
                (cg.IsNotPresentForm ?? false) == false

            orderby bc.SortOrd, cg.ClassName

            select new
            {
                ClassGroup = cg,
                ClassType = ctype,
                BasicClass = bc,
                ClassBook = cb
            }
        ).ToArrayAsync(ct);

        var parentClassGroups = classGroups
            .Where(cg => cg.ClassGroup.ParentClassId == null)
            .ToLookup(cg => cg.ClassGroup.ClassId);

        var childClassGroups = classGroups
            .Where(cg => cg.ClassGroup.ParentClassId != null)
            .ToLookup(cg => cg.ClassGroup.ParentClassId);

        return classGroups
            .Where(cg =>
            {
                var isLvl1 = cg.ClassGroup.ParentClassId == null;
                var isCombined = cg.ClassGroup.IsCombined == true;
                var hasIsCombinedParent =
                    cg.ClassGroup.ParentClassId != null &&
                    parentClassGroups[cg.ClassGroup.ParentClassId.Value].Any(p => p.ClassGroup.IsCombined == true);

                // leave only:
                // lvl1 regular
                // lvl1 combined (for visualization only)
                // lvl2 combined
                // lvl2 not combined with combined parent (for visualization only as they are invalid for creation)

                return isLvl1 || isCombined || hasIsCombinedParent;
            })
            .Select(cg =>
            {
                var (classBookType, classBookTypeError) = cg.ClassBook?.BookType != null ?
                    new (cg.ClassBook?.BookType, null) :
                    ClassBook.GetClassBookTypeForClassGroup(
                            instType,
                            cg.ClassGroup,
                            cg.ClassType,
                            childClassGroups[cg.ClassGroup.ClassId]
                                .Select(ccg => (ccg.ClassGroup, (ClassType?)ccg.ClassType))
                                .ToArray());

                return new GetAllForClassKindVO(
                    cg.ClassGroup.ClassId,
                    cg.ClassGroup.ClassName,
                    cg.ClassGroup.ParentClassId,
                    cg.BasicClass?.Name,
                    cg.ClassType?.Name,
                    cg.ClassGroup.ParentClassId == null && cg.ClassGroup.IsCombined == true,
                    cg.ClassGroup.ParentClassId != null,
                    cg.ClassBook?.ClassBookId,
                    cg.ClassBook?.BookName,
                    classBookType,
                    classBookTypeError,
                    cg.ClassBook == null ? ClassBook.GetSuggestedClassBookNameForClassGroup(cg.ClassGroup, cg.BasicClass) : null);
            })
            .ToArray();
    }

    public async Task<GetAllForClassKindVO[]> GetAllForReorganizeAsync(
        int schoolYear,
        int instId,
        ClassKind classKind,
        ClassBookReorganizeType reorganizeType,
        CancellationToken ct)
    {
        var classGroupPredicate = PredicateBuilder.True<ClassGroup>();
        classGroupPredicate = classGroupPredicate.And(cg => cg.IsCombined == true);

        var instType = await (
            from i in this.DbContext.Set<InstitutionSchoolYear>()

            join d in this.DbContext.Set<DetailedSchoolType>() on i.DetailedSchoolTypeId equals d.DetailedSchoolTypeId

            where i.SchoolYear == schoolYear &&
                i.InstitutionId == instId

            select d.InstType
        ).SingleAsync(ct);

        var classGroups = await (
            from cg in this.DbContext.Set<ClassGroup>().Where(classGroupPredicate)

            join ctype in this.DbContext.Set<ClassType>() on cg.ClassTypeId equals ctype.ClassTypeId
            into j1
            from ctype in j1.DefaultIfEmpty()

            join bc in this.DbContext.Set<BasicClass>() on cg.BasicClassId equals bc.BasicClassId
            into j2
            from bc in j2.DefaultIfEmpty()

            join cb in this.DbContext.Set<ClassBook>().Where(cb => cb.IsValid) on new { cg.SchoolYear, cg.ClassId } equals new { cb.SchoolYear, cb.ClassId }
            into j3
            from cb in j3.DefaultIfEmpty()

            where cg.SchoolYear == schoolYear &&
                cg.InstitutionId == instId &&
                cg.IsValid &&
                ((ClassKind?)ctype.ClassKind ?? ClassKind.Class) == classKind &&
                // exclude not present form (IFO) system class
                (cg.IsNotPresentForm ?? false) == false

            orderby bc.SortOrd, cg.ClassName

            select new
            {
                ClassGroup = cg,
                ClassType = ctype,
                BasicClass = bc,
                ClassBook = cb
            }
        ).ToArrayAsync(ct);

        var parentClassGroups = classGroups
            .Where(cg => cg.ClassGroup.ParentClassId == null)
            .ToLookup(cg => cg.ClassGroup.ClassId);

        var childClassGroups = classGroups
            .Where(cg => cg.ClassGroup.ParentClassId != null)
            .ToLookup(cg => cg.ClassGroup.ParentClassId);

        // filter classGroups to return only values that can be reorganize
        if (reorganizeType == ClassBookReorganizeType.Combine)
        {
            var classIds = parentClassGroups
                .Where(pcg => childClassGroups[pcg.Key].Any(ccg => ccg.ClassBook != null))
                .Select(pcg => pcg.Key);

            classGroups = classGroups
                .Where(cg => classIds.Contains(cg.ClassGroup.ClassId) || (cg.ClassGroup.ParentClassId != null && classIds.Contains((int)cg.ClassGroup.ParentClassId)))
                .ToArray();
        }
        else
        {
            var classIds = classGroups
                .Where(cg =>
                    cg.ClassGroup.ParentClassId == null &&
                    cg.ClassBook != null &&
                    childClassGroups[cg.ClassGroup.ClassId].All(ccg => ccg.ClassBook == null))
                .Select(cg => cg.ClassGroup.ClassId);

            classGroups = classGroups
                .Where(cg => classIds.Contains(cg.ClassGroup.ClassId) || (cg.ClassGroup.ParentClassId != null && classIds.Contains((int)cg.ClassGroup.ParentClassId)))
                .ToArray();
        }

        return classGroups
            .Select(cg =>
            {
                var (classBookType, classBookTypeError) = cg.ClassBook?.BookType != null ?
                    new(cg.ClassBook?.BookType, null) :
                    ClassBook.GetClassBookTypeForClassGroup(
                            instType,
                            cg.ClassGroup,
                            cg.ClassType,
                            childClassGroups[cg.ClassGroup.ClassId]
                                .Select(ccg => (ccg.ClassGroup, (ClassType?)ccg.ClassType))
                                .ToArray());

                return new GetAllForClassKindVO(
                    cg.ClassGroup.ClassId,
                    cg.ClassGroup.ClassName,
                    cg.ClassGroup.ParentClassId,
                    cg.BasicClass?.Name,
                    cg.ClassType?.Name,
                    cg.ClassGroup.ParentClassId == null,
                    cg.ClassGroup.ParentClassId != null,
                    cg.ClassBook?.ClassBookId,
                    cg.ClassBook?.BookName,
                    classBookType,
                    classBookTypeError,
                    cg.ClassBook == null ? ClassBook.GetSuggestedClassBookNameForClassGroup(cg.ClassGroup, cg.BasicClass) : null);
            })
            .ToArray();
    }

    public async Task<GetExtApiClassGroupsVO[]> GetClassGroupsAsync(
        int schoolYear,
        int instId,
        int[] classIds,
        CancellationToken ct)
    {
        var classGroups = await (
            from cg in this.DbContext.Set<ClassGroup>()

            join ctype in this.DbContext.Set<ClassType>() on cg.ClassTypeId equals ctype.ClassTypeId
            into j1 from ctype in j1.DefaultIfEmpty()

            join bc in this.DbContext.Set<BasicClass>() on cg.BasicClassId equals bc.BasicClassId
            into j2 from bc in j2.DefaultIfEmpty()

            where cg.SchoolYear == schoolYear &&
                cg.InstitutionId == instId &&
                cg.IsValid &&
                this.DbContext
                    .MakeIdsQuery(classIds)
                    .Any(id => cg.ClassId == id.Id || cg.ParentClassId == id.Id)

            select new
            {
                ClassGroup = cg,
                BasicClass = bc,
                ClassType = ctype,
            }
        ).ToArrayAsync(ct);

        return classGroups
            .Select(cg =>
                new GetExtApiClassGroupsVO(
                    cg.ClassGroup,
                    cg.BasicClass,
                    cg.ClassType))
            .ToArray();
    }

    public async Task<GetClassGroupsWIthClassBookVO[]> GetClassGroupsWithClassBookAsync(
        int schoolYear,
        int instId,
        int[] classIds,
        CancellationToken ct)
    {
        return await (
            from cg in this.DbContext.Set<ClassGroup>()

            join cb in this.DbContext.Set<ClassBook>().Where(cb => cb.IsValid) on new { cg.SchoolYear, cg.ClassId } equals new { cb.SchoolYear, cb.ClassId }
                into j3
            from cb in j3.DefaultIfEmpty()

            where cg.SchoolYear == schoolYear &&
                  cg.InstitutionId == instId &&
                  cg.IsValid &&
                  this.DbContext
                      .MakeIdsQuery(classIds)
                      .Any(id => cg.ClassId == id.Id) &&
                  // exclude not present form (IFO) system class
                  (cg.IsNotPresentForm ?? false) == false

            select new
            {
                ClassGroup = cg,
                ClassBook = cb
            }
        )
        .Select(cg => new GetClassGroupsWIthClassBookVO(cg.ClassGroup, cg.ClassBook))
        .ToArrayAsync(ct);
    }

    public async Task<GetInvalidClassGroupsForClassBookCreationVO[]> GetInvalidClassGroupsForClassBookCreationAsync(
        int schoolYear,
        int instId,
        int[] classIds,
        CancellationToken ct)
    {
        var instType = await (
            from i in this.DbContext.Set<InstitutionSchoolYear>()

            join d in this.DbContext.Set<DetailedSchoolType>() on i.DetailedSchoolTypeId equals d.DetailedSchoolTypeId

            where i.SchoolYear == schoolYear &&
                i.InstitutionId == instId

            select d.InstType
        ).SingleAsync(ct);

        var classGroups = await (
            from cg in this.DbContext.Set<ClassGroup>()

            join ctype in this.DbContext.Set<ClassType>() on cg.ClassTypeId equals ctype.ClassTypeId
            into j1 from ctype in j1.DefaultIfEmpty()

            join bc in this.DbContext.Set<BasicClass>() on cg.BasicClassId equals bc.BasicClassId
            into j2 from bc in j2.DefaultIfEmpty()

            join cb in this.DbContext.Set<ClassBook>().Where(cb => cb.IsValid) on new { cg.SchoolYear, cg.ClassId } equals new { cb.SchoolYear, cb.ClassId }
            into j3 from cb in j3.DefaultIfEmpty()

            where cg.SchoolYear == schoolYear &&
                cg.InstitutionId == instId

            select new
            {
                ClassGroup = cg,
                ClassType = ctype,
                ClassBook = cb
            }
        ).ToArrayAsync(ct);

        var classGroupsDict = classGroups.ToDictionary(cg => cg.ClassGroup.ClassId);

        var childClassGroups = classGroups
            .Where(cg => cg.ClassGroup.ParentClassId != null)
            .ToLookup(cg => cg.ClassGroup.ParentClassId);

        var parentClassGroups = classGroups
            .Where(cg => cg.ClassGroup.ParentClassId == null)
            .ToLookup(cg => cg.ClassGroup.ClassId);

        return classIds
            .Select(classId =>
            {
                var cg = classGroupsDict.GetValueOrDefault(classId);

                ClassBookTypeError? classBookTypeError = null;
                bool hasClassBookOnParentOrChildLevel = false;

                if (cg != null)
                {
                    (_, classBookTypeError) =
                        ClassBook.GetClassBookTypeForClassGroup(
                            instType,
                            cg.ClassGroup,
                            cg.ClassType,
                            childClassGroups[cg.ClassGroup.ClassId]
                                .Select(ccg => (ccg.ClassGroup, (ClassType?)ccg.ClassType))
                                .ToArray());

                    hasClassBookOnParentOrChildLevel =
                        cg.ClassGroup.ParentClassId != null ?
                            parentClassGroups[cg.ClassGroup.ParentClassId.Value].Where(pcg => pcg.ClassBook != null).Any() :
                            childClassGroups[cg.ClassGroup.ClassId].Where(ccg => ccg.ClassBook != null).Any();
                }

                return new
                {
                    ClassId = classId,
                    cg?.ClassGroup.ClassName,
                    cg?.ClassGroup.IsValid,
                    cg?.ClassBook?.ClassBookId,
                    ClassBookTypeError = classBookTypeError,
                    IsNonexistentClassGroup = cg == null,
                    HasClassBookOnParentOrChildLevel = hasClassBookOnParentOrChildLevel
                };
            })
            .Where(cg =>
                cg.IsValid == false ||
                cg.ClassBookId != null ||
                cg.ClassBookTypeError != null ||
                cg.IsNonexistentClassGroup ||
                cg.HasClassBookOnParentOrChildLevel)
            .Select(cg => new GetInvalidClassGroupsForClassBookCreationVO(
                cg.ClassId,
                cg.ClassName,
                cg.IsNonexistentClassGroup,
                cg.IsValid == false,
                cg.ClassBookId != null,
                cg.HasClassBookOnParentOrChildLevel,
                cg.ClassBookTypeError
            ))
            .ToArray();
    }

    public async Task<InstType> GetInstTypeAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        return await (
            from i in this.DbContext.Set<InstitutionSchoolYear>()

            join d in this.DbContext.Set<DetailedSchoolType>() on i.DetailedSchoolTypeId equals d.DetailedSchoolTypeId

            where i.SchoolYear == schoolYear &&
                i.InstitutionId == instId

            select d.InstType
        ).SingleAsync(ct);
    }
}
