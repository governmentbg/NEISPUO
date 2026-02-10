namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CombineClassBooksExtApiCommandHandler(
    IClassBookReorganizeService ClassBookReorganizeService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CombineClassBooksExtApiCommand, int>
{
    public async Task<int> Handle(CombineClassBooksExtApiCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
                command.SchoolYear!.Value,
                command.InstId!.Value,
                ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        return await this.ClassBookReorganizeService.CombineClassBooks(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ParentClassId!.Value,
            command.ParentClassBookName,
            command.ChildClassIdForDataTransfer!.Value,
            command.SysUserId!.Value,
            ct
        );
    }
}
