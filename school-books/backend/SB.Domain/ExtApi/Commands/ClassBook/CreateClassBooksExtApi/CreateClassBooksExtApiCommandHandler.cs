namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateClassBooksExtApiCommandHandler(
    IClassBookService ClassBookService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateClassBooksExtApiCommand, int[]>
{
    public async Task<int[]> Handle(CreateClassBooksExtApiCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        return await this.ClassBookService.CreateClassBooks(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ClassIds!
                .Select(classId => (classId, classBookName: (string?)null))
                .ToArray(),
            command.SysUserId!.Value,
            ct
        );
    }
}
