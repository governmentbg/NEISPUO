namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateRegularGradePointAverageByClassesReportCommandHandler(
        IUnitOfWork UnitOfWork,
        IRegularGradePointAverageByClassesReportsAggregateRepository RegularGradePointAverageByClassesReportsAggregateRepository,
        IRegularGradePointAverageByClassesReportsQueryRepository RegularGradePointAverageByClassesReportsQueryRepository,
        IClassBooksQueryRepository ClassBooksQueryRepository)
    : IRequestHandler<CreateRegularGradePointAverageByClassesReportCommand, int>
{
    public async Task<int> Handle(CreateRegularGradePointAverageByClassesReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var classBookIds = command.ClassBookIds ?? Array.Empty<int>();

        var items = await this.RegularGradePointAverageByClassesReportsQueryRepository.GetItemsForAddAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            classBookIds,
            ct);

        var classBooks = Array.Empty<string>();

        if (classBookIds.Any())
        {
            classBooks = await this.ClassBooksQueryRepository.GetClassBookNamesByIdsAsync(command.SchoolYear!.Value, command.InstId!.Value, classBookIds, ct);
        }

        RegularGradePointAverageByClassesReport regularGradePointAverageByClassesReport = new(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            string.Join(", ", classBooks),
            items,
            createDate,
            command.SysUserId!.Value);

        await this.RegularGradePointAverageByClassesReportsAggregateRepository.AddAsync(regularGradePointAverageByClassesReport, ct);

        if (regularGradePointAverageByClassesReport.Items.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { regularGradePointAverageByClassesReport },
                ct);

            await this.UnitOfWork.BulkInsertAsync(
                regularGradePointAverageByClassesReport.Items,
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }

        return regularGradePointAverageByClassesReport.RegularGradePointAverageByClassesReportId;
    }
}
