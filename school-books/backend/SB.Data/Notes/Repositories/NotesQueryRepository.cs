namespace SB.Data;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.INotesQueryRepository;

internal class NotesQueryRepository : Repository, INotesQueryRepository
{
    public NotesQueryRepository(UnitOfWork unitOfWork)
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
        var students = (await (
            from n in this.DbContext.Set<Note>()

            join st in this.DbContext.Set<NoteStudent>()
                on new { n.SchoolYear, n.NoteId }
                equals new { st.SchoolYear, st.NoteId }
            join p in this.DbContext.Set<Person>()
                on st.PersonId
                equals p.PersonId

            where n.SchoolYear == schoolYear &&
                n.ClassBookId == classBookId

            orderby p.FirstName, p.MiddleName, p.LastName

            select new
            {
                n.NoteId,
                p.FirstName,
                p.MiddleName,
                p.LastName
            })
            .ToArrayAsync(ct))
            .ToLookup(
                ss => ss.NoteId,
                ss => StringUtils.JoinNames(ss.FirstName, ss.MiddleName, ss.LastName));

        return await(
            from n in this.DbContext.Set<Note>()

            where n.SchoolYear == schoolYear &&
                n.ClassBookId == classBookId

            orderby n.CreateDate descending

            select new GetAllVO(
                n.NoteId,
                string.Join(", ", students[n.NoteId]),
                n.Description,
                n.IsForAllStudents))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int noteId,
        CancellationToken ct)
    {
        return await (
            from n in this.DbContext.Set<Note>()

            where n.SchoolYear == schoolYear &&
                n.ClassBookId == classBookId &&
                n.NoteId == noteId

            orderby n.NoteId

            select new GetVO(
                n.NoteId,
                n.Students.Select(s => s.PersonId).ToArray(),
                n.Description,
                n.IsForAllStudents,
                n.CreatedBySysUserId))
            .SingleAsync(ct);
    }
}
