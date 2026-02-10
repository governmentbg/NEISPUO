namespace SB.Domain;

using MediatR;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.ITopicPlanItemsExcelReaderWriter;

internal record CreateClassBookTopicPlanItemsFromExcelImportCommandHandler(
    IUnitOfWork UnitOfWork,
    IClassBookTopicPlanItemsQueryRepository ClassBookTopicPlanItemsQueryRepository,
    IClassBookTopicPlanItemsAggregateRepository ClassBookTopicPlanItemsAggregateRepository,
    BlobServiceClient BlobServiceClient,
    ITopicPlanItemsExcelReaderWriter TopicPlanItemsExcelReaderWriter,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateClassBookTopicPlanItemsFromExcelImportCommand, int[]>
{
    public async Task<int[]> Handle(CreateClassBookTopicPlanItemsFromExcelImportCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int classBookId = command.ClassBookId!.Value;
        int sysUserId = command.SysUserId!.Value;
        int curriculumId = command.CurriculumId!.Value;
        int blobId = command.BlobId!.Value;

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

        if (!await this.ClassBookCachedQueryStore.ExistsCurriculumForClassBookAsync(
            schoolYear,
            classBookId,
            curriculumId,
            ct))
        {
            throw new DomainValidationException($"This curriculum is not in the class book curriculum list");
        }

        if (await this.ClassBookTopicPlanItemsQueryRepository.HasAnyAsync(
            schoolYear,
            classBookId,
            curriculumId,
            ct))
        {
            throw new DomainValidationException(new[] { "has_existing_topic_plan" }, new[] { "Не може да импортирате ново тематично разпределение преди да изтриете старото." });
        }

        using var ms = new MemoryStream();

        await this.BlobServiceClient.DownloadBlobToStreamAsync(blobId, ms, ct);

        var errors = this.TopicPlanItemsExcelReaderWriter.TryRead(ms, out TopicPlanItemDO[] items);

        if (errors.Any())
        {
            throw new DomainValidationException(new[] { "has_import_errors" }, errors);
        }

        List<int> classBookTopicPlanItemIds = new();
        foreach (var item in items)
        {
            ClassBookTopicPlanItem classBookTopicPlanItem =
                new(schoolYear,
                    classBookId,
                    curriculumId,
                    item.Number,
                    item.Title,
                    item.Note,
                    false,
                    sysUserId);
            await this.ClassBookTopicPlanItemsAggregateRepository.AddAsync(
                classBookTopicPlanItem,
                ct);

            classBookTopicPlanItemIds.Add(classBookTopicPlanItem.ClassBookTopicPlanItemId);
        }

        await this.UnitOfWork.SaveAsync(ct);

        return classBookTopicPlanItemIds.ToArray();
    }
}
