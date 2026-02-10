namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Xunit;

public sealed class DataFixture_PG : IAsyncLifetime
{
    private const int schoolYear = 2025;
    private const int institutionId = 300125;

    public async Task InitializeAsync()
    {
        await using var connection = new SqlConnection(DatabaseUtils.ConnectionString);
        (this.ClassIdOnChildLevel, this.ClassIdOnParentLevel, this.IsLevel2Class) = await DataQueries.ClassIdOnChildLevelOrDefaultAsync(connection, schoolYear, institutionId);

        var pgStudentDataIds =
            (await DataQueries.GetStudentDataIdsAsync(connection, schoolYear, institutionId, ClassBookType.Book_PG))
            .GroupBy(x => x.ClassBookId)
            .Where(r => r.Where(s => !s.IsTransferred).DistinctBy(s => s.PersonId).Count() >= 3)
            .FirstOrDefault();

        if (pgStudentDataIds != null)
        {
            this.ClassBookId = pgStudentDataIds.Key;
            this.AllClassBookStudentClasses = pgStudentDataIds
                .Select(s => (
                    classId: s.ClassId,
                    personId: s.PersonId,
                    isTransferred: s.IsTransferred
                )).ToArray();

            var distinctEnrolledPgStudentDataIds = pgStudentDataIds.Where(s => !s.IsTransferred).DistinctBy(s => s.PersonId);
            this.ClassId = distinctEnrolledPgStudentDataIds.ElementAt(0).ClassId;
            this.PersonId = distinctEnrolledPgStudentDataIds.ElementAt(0).PersonId;
            this.PersonIdUpdate = distinctEnrolledPgStudentDataIds.ElementAt(1).PersonId;
            this.PersonIdRemove = distinctEnrolledPgStudentDataIds.ElementAt(2).PersonId;
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public int SchoolYear => schoolYear;

    public int InstitutionId => institutionId;

    public int ClassBookId { get; private set; }

    public int ClassIdOnChildLevel { get; private set; }

    public int ClassIdOnParentLevel { get; private set; }

    public bool IsLevel2Class { get; private set; }

    public (int classId, int personId, bool isTransferred)[] AllClassBookStudentClasses { get; private set; } = Array.Empty<(int classId, int personId, bool isTransferred)>();

    public int ClassId { get; private set; }

    public int PersonId { get; private set; }

    public int PersonIdUpdate { get; private set; }

    public int PersonIdRemove { get; private set; }
}
