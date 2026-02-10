namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateFinalGradePointAverageByStudentsReportCommandHandler(
        IUnitOfWork UnitOfWork,
        IFinalGradePointAverageByStudentsReportsAggregateRepository FinalGradePointAverageByStudentsReportsAggregateRepository,
        IFinalGradePointAverageByStudentsReportsQueryRepository FinalGradePointAverageByStudentsReportsQueryRepository,
        IClassBooksQueryRepository ClassBooksQueryRepository)
    : IRequestHandler<CreateFinalGradePointAverageByStudentsReportCommand, int>
{
    public async Task<int> Handle(CreateFinalGradePointAverageByStudentsReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var classBookIds = command.ClassBookIds ?? Array.Empty<int>();

        var items = await this.FinalGradePointAverageByStudentsReportsQueryRepository.GetItemsForAddAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            classBookIds,
            command.MinimumGradePointAverage,
            ct);

        var classBooks = Array.Empty<string>();

        if (classBookIds.Any())
        {
            classBooks = await this.ClassBooksQueryRepository.GetClassBookNamesByIdsAsync(command.SchoolYear!.Value, command.InstId!.Value, classBookIds, ct);
        }

        FinalGradePointAverageByStudentsReport finalGradePointAverageByStudentsReport = new(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            string.Join(", ", classBooks),
            command.MinimumGradePointAverage,
            items,
            createDate,
            command.SysUserId!.Value);

        await this.FinalGradePointAverageByStudentsReportsAggregateRepository.AddAsync(finalGradePointAverageByStudentsReport, ct);

        if (finalGradePointAverageByStudentsReport.Items.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { finalGradePointAverageByStudentsReport },
                ct);

            await this.UnitOfWork.BulkInsertAsync(
                finalGradePointAverageByStudentsReport.Items,
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }

        return finalGradePointAverageByStudentsReport.FinalGradePointAverageByStudentsReportId;
    }
}
