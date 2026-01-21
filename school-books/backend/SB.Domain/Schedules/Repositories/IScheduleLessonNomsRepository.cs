namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IScheduleLessonNomsRepository
{
    Task<ScheduleLessonNomVO[]> GetNomsAsync(int schoolYear, int instId, int classBookId, int curriculumId, DateTime date, int? curriculumTeacherPersonId, CancellationToken ct);
}
