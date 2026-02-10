namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateClassBookCommandHandler(
    IClassBookService ClassBookService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateClassBookCommand, int[]>
{
    public async Task<int[]> Handle(CreateClassBookCommand command, CancellationToken ct)
    {
        if (command.ClassBooks == null)
        {
            return Array.Empty<int>();
        }

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
            command.ClassBooks!
                .Select(cb =>
                    (classId: cb.ClassId!.Value, classBookName: (string?)(cb.ClassBookName ?? "")))
                .ToArray(),
            command.SysUserId!.Value,
            ct
        );
    }
}
