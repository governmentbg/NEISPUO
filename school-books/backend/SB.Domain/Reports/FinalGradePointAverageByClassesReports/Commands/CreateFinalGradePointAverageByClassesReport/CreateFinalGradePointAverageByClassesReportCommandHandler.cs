namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateFinalGradePointAverageByClassesReportCommandHandler(
        IUnitOfWork UnitOfWork,
        IFinalGradePointAverageByClassesReportsAggregateRepository FinalGradePointAverageByClassesReportsAggregateRepository,
        IFinalGradePointAverageByClassesReportsQueryRepository FinalGradePointAverageByClassesReportsQueryRepository,
        IClassBooksQueryRepository ClassBooksQueryRepository)
    : IRequestHandler<CreateFinalGradePointAverageByClassesReportCommand, int>
{
    public async Task<int> Handle(CreateFinalGradePointAverageByClassesReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var classBookIds = command.ClassBookIds ?? Array.Empty<int>();

        var items = await this.FinalGradePointAverageByClassesReportsQueryRepository.GetItemsForAddAsync(
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

        FinalGradePointAverageByClassesReport finalGradePointAverageByClassesReport = new(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            string.Join(", ", classBooks),
            items,
            createDate,
            command.SysUserId!.Value);

        await this.FinalGradePointAverageByClassesReportsAggregateRepository.AddAsync(finalGradePointAverageByClassesReport, ct);

        if (finalGradePointAverageByClassesReport.Items.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { finalGradePointAverageByClassesReport },
                ct);

            await this.UnitOfWork.BulkInsertAsync(
                finalGradePointAverageByClassesReport.Items,
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }

        return finalGradePointAverageByClassesReport.FinalGradePointAverageByClassesReportId;
    }
}
