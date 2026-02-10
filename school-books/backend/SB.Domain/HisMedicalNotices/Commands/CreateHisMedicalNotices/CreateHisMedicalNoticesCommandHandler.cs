namespace SB.Domain;

using System;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Medallion.Threading;
using MediatR;

internal record CreateHisMedicalNoticesCommandHandler(
    Func<string, IDbTransaction, bool, IDistributedLock> LockFactory,
    IUnitOfWork UnitOfWork,
    IAggregateRepository<HisMedicalNoticeBatch> HisMedicalNoticeBatchAggregateRepository,
    IAggregateRepository<HisMedicalNotice> HisMedicalNoticeAggregateRepository,
    IRequestContext RequestContext,
    IServiceProvider ServiceProvider)
    : IRequestHandler<CreateHisMedicalNoticesCommand>
{
    public async Task Handle(CreateHisMedicalNoticesCommand command, CancellationToken ct)
    {
        // This should never throw an exception or else we will lose the error and not create a batch.
        // Email reference -> "Промяна в протокола за изпращане на медицински бележки между НЗИС и НЕИСПУО" - 01.11.2024
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8601 // Possible null reference assignment.
        if (command?.MedicalNotices != null)
        {
            command = command with
            {
                MedicalNotices = command.MedicalNotices?.Select(mn =>
                {
                    if (mn == null)
                    {
                        return null;
                    }

                    return mn with
                    {
                        Patient =
                        mn.Patient != null
                            ? mn.Patient with
                            {
                                GivenName = string.IsNullOrEmpty(mn.Patient.GivenName) ? " " : mn.Patient.GivenName,
                                FamilyName = string.IsNullOrEmpty(mn.Patient.FamilyName) ? " " : mn.Patient.FamilyName,
                            }
                            : null
                    };
                }).ToArray()
            };
        }
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

        (bool isValid, string[] errors, string[] errorMessages) = await this.ValidateCommandAsync(command!, ct);

        HisMedicalNoticeBatch batch = new(
            this.RequestContext.RequestId,
            isValid
                ? "Uknown error" // we'll clear that at the end if everything goes well
                : JsonSerializer.Serialize(new { Errors = errors, ErrorMessages = errorMessages }));
        await this.HisMedicalNoticeBatchAggregateRepository.AddAsync(batch, ct);
        await this.UnitOfWork.SaveAsync(ct);

        if (!isValid)
        {
            throw new DomainValidationException(
                errors,
                errorMessages,
                "");
        }

        await using var transaction = await this.UnitOfWork.BeginTransactionAsync(ct);

        using var lockReleaseHandle = await this.TryAcquireDistributedLockAsync(
            "CreateHisMedicalNotices",
            transaction.GetDbTransaction(),
            true,
            TimeSpan.FromSeconds(30),
            ct);

        if (lockReleaseHandle == null)
        {
            // we couldn't take the lock
            batch.SetError("Lock timeout");
            await this.UnitOfWork.SaveAsync(ct);
            await transaction.CommitAsync(ct);

            throw new Exception("Could not acquire lock");
        }

        foreach (var mn in command!.MedicalNotices!)
        {
            HisMedicalNotice hisMedicalNotice = new(
                batch.HisMedicalNoticeBatchId,
                batch.CreateDate,
                mn.NrnMedicalNotice,
                mn.NrnExamination,
                mn.Patient.IdentifierType,
                mn.Patient.Identifier,
                mn.Patient.GivenName,
                mn.Patient.FamilyName,
                mn.Practitioner.Pmi,
                mn.MedicalNotice.FromDate,
                mn.MedicalNotice.ToDate,
                mn.MedicalNotice.AuthoredOn);

            await this.HisMedicalNoticeAggregateRepository.AddAsync(
                entity: hisMedicalNotice,
                preventDetectChanges: true,
                ct: ct);
        }

        batch.SetError(null);
        await this.UnitOfWork.SaveAsync(ct);
        await transaction.CommitAsync(ct);
    }

    private async Task<(bool isValid, string[] errors, string[] errorMessages)>
        ValidateCommandAsync(CreateHisMedicalNoticesCommand command, CancellationToken ct)
    {
        var context = new ValidationContext<CreateHisMedicalNoticesCommand>(command);
        context.SetServiceProvider(this.ServiceProvider);

        var validator = new CreateHisMedicalNoticesCommandValidator();
        var validationResult = await validator.ValidateAsync(context, ct);

        if (validationResult.Errors.Count > 0)
        {
            return (isValid:false,
                errors: validationResult.Errors.Where(f => f.ErrorCode != ValidationExtensions.UserErrorCode).Select(f => f.ErrorMessage).ToArray(),
                errorMessages: validationResult.Errors.Where(f => f.ErrorCode == ValidationExtensions.UserErrorCode).Select(f => f.ErrorMessage).ToArray());
        }

        return (isValid: true, errors: Array.Empty<string>(), errorMessages: Array.Empty<string>());
    }

    private ValueTask<IDistributedSynchronizationHandle?> TryAcquireDistributedLockAsync(
        string name,
        IDbTransaction transaction,
        bool exactName,
        TimeSpan timeout,
        CancellationToken ct)
        => this.LockFactory(name, transaction, exactName).TryAcquireAsync(timeout, ct);
}
