namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateRemarkExtApiCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Remark> RemarkAggregateRepository,
    INotificationsQueryRepository NotificationsQueryRepository,
    INotificationsService NotificationsService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateRemarkExtApiCommand, int>
{
    public async Task<int> Handle(CreateRemarkExtApiCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int classBookId = command.ClassBookId!.Value;

        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            schoolYear,
            classBookId,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{schoolYear}, classBookId:{classBookId}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            schoolYear,
            classBookId,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
            schoolYear,
            classBookId,
            command.PersonId!.Value,
            ct))
        {
            throw new DomainValidationException("This person is not in the class book students list");
        }

        if (!await this.ClassBookCachedQueryStore.ExistsCurriculumForClassBookAsync(
                schoolYear,
                classBookId,
                command.CurriculumId!.Value,
                ct))
        {
            throw new DomainValidationException($"This curriculum is not in the class book curriculum list");
        }

        var remark = new Remark(
            schoolYear,
            classBookId,
            command.PersonId!.Value,
            command.Type!.Value,
            command.CurriculumId!.Value,
            command.Date!.Value,
            command.Description!,
            command.SysUserId!.Value);

        await this.RemarkAggregateRepository.AddAsync(remark, ct);

        await this.UnitOfWork.SaveAsync(ct);

        return remark.RemarkId;
    }
}
