namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record SeparateClassBooksExtApiCommandHandler(
    IClassBookReorganizeService ClassBookReorganizeService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<SeparateClassBooksExtApiCommand, int[]>
{
    public async Task<int[]> Handle(SeparateClassBooksExtApiCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
                command.SchoolYear!.Value,
                command.InstId!.Value,
                ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        return await this.ClassBookReorganizeService.SeparateClassBooks(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ParentClassId!.Value,
            command.ChildClassBooks!
                .Select(cb =>
                    (classId: cb.ClassId!.Value, classBookName: (string?)(cb.ClassBookName ?? "")))
                .ToArray(),
            command.SysUserId!.Value,
            ct
        );
    }
}
