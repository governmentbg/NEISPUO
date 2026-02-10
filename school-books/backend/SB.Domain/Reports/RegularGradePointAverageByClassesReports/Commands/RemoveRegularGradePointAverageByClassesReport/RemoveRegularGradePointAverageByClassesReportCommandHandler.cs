namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveRegularGradePointAverageByClassesReportCommandHandler(
        IRegularGradePointAverageByClassesReportsAggregateRepository RegularGradePointAverageByClassesReportsAggregateRepository)
    : IRequestHandler<RemoveRegularGradePointAverageByClassesReportCommand>
{
    public async Task Handle(RemoveRegularGradePointAverageByClassesReportCommand command, CancellationToken ct)
    {
        await this.RegularGradePointAverageByClassesReportsAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.RegularGradePointAverageByClassesReportId!.Value,
            ct);
    }
}
