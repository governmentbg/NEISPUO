namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveRegularGradePointAverageByStudentsReportCommandHandler(
        IRegularGradePointAverageByStudentsReportsAggregateRepository RegularGradePointAverageByStudentsReportsAggregateRepository)
    : IRequestHandler<RemoveRegularGradePointAverageByStudentsReportCommand>
{
    public async Task Handle(RemoveRegularGradePointAverageByStudentsReportCommand command, CancellationToken ct)
    {
        await this.RegularGradePointAverageByStudentsReportsAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.RegularGradePointAverageByStudentsReportId!.Value,
            ct);
    }
}
