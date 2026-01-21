namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateClassBookExtApiCommandHandler(
    IClassBookService ClassBookService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateClassBookExtApiCommand, int>
{
    public async Task<int> Handle(CreateClassBookExtApiCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var classBookIds = await this.ClassBookService.CreateClassBooks(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            new [] { (classId: command.ClassId!.Value, classBookName: (string?)null) },
            command.SysUserId!.Value,
            ct
        );

        return classBookIds.Single();
    }
}
