namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateRegularGradePointAverageByStudentsReportCommandHandler(
        IUnitOfWork UnitOfWork,
        IRegularGradePointAverageByStudentsReportsAggregateRepository RegularGradePointAverageByStudentsReportsAggregateRepository,
        IRegularGradePointAverageByStudentsReportsQueryRepository RegularGradePointAverageByStudentsReportsQueryRepository,
        IClassBooksQueryRepository ClassBooksQueryRepository)
    : IRequestHandler<CreateRegularGradePointAverageByStudentsReportCommand, int>
{
    public async Task<int> Handle(CreateRegularGradePointAverageByStudentsReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var classBookIds = command.ClassBookIds ?? Array.Empty<int>();

        var items = await this.RegularGradePointAverageByStudentsReportsQueryRepository.GetItemsForAddAsync(
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

        RegularGradePointAverageByStudentsReport regularGradePointAverageByStudentsReport = new(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            string.Join(", ", classBooks),
            items,
            createDate,
            command.SysUserId!.Value);

        await this.RegularGradePointAverageByStudentsReportsAggregateRepository.AddAsync(regularGradePointAverageByStudentsReport, ct);

        if (regularGradePointAverageByStudentsReport.Items.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { regularGradePointAverageByStudentsReport },
                ct);

            await this.UnitOfWork.BulkInsertAsync(
                regularGradePointAverageByStudentsReport.Items,
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }

        return regularGradePointAverageByStudentsReport.RegularGradePointAverageByStudentsReportId;
    }
}
