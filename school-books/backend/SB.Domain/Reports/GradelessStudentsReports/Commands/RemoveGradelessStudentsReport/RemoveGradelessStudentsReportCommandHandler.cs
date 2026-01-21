namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveGradelessStudentsReportCommandHandler(
    IGradelessStudentsReportsAggregateRepository GradelessStudentsReportsAggregateRepository)
    : IRequestHandler<RemoveGradelessStudentsReportCommand>
{
    public async Task Handle(RemoveGradelessStudentsReportCommand command, CancellationToken ct)
    {
        await this.GradelessStudentsReportsAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.GradelessStudentsReportId!.Value,
            ct);
    }
}
