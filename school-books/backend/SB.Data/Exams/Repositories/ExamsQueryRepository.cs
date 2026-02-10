namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IExamsQueryRepository;

internal class ExamsQueryRepository : Repository, IExamsQueryRepository
{
    public ExamsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int classBookId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from e in this.DbContext.Set<Exam>()

            join c in this.DbContext.Set<Curriculum>() on e.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where e.SchoolYear == schoolYear &&
                e.ClassBookId == classBookId

            orderby e.Date descending

            select new GetAllVO(
                e.ExamId,
                e.Type,
                $"{s.SubjectName} / {st.Name}" +
                (c.IsIndividualCurriculum == true ? " (ИУП)" : "") +
                (c.IsIndividualLesson == 1 ? " (ИЧ)" : ""),
                e.Date))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int examId,
        CancellationToken ct)
    {
        return await (
            from e in this.DbContext.Set<Exam>()

            where  e.SchoolYear == schoolYear &&
                e.ClassBookId == classBookId &&
                e.ExamId == examId

            select new GetVO(
                e.ExamId,
                e.CurriculumId,
                e.Date,
                e.Description))
            .SingleAsync(ct);
    }
}
