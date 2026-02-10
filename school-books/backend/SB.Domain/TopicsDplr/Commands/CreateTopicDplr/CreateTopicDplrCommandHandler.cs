namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateTopicDplrCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<TopicDplr> TopicAggregateRepository,
    ITopicsDplrQueryRepository TopicsDplrQueryRepository,
    IClassBookTopicPlanItemsAggregateRepository ClassBookTopicPlanItemsAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateTopicDplrCommand, int>
{
    public async Task<int> Handle(CreateTopicDplrCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.ExistsCurriculumForClassBookAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                command.CurriculumId!.Value,
                ct))
        {
            throw new DomainValidationException($"This curriculum ({command.CurriculumId.Value}) is not in the class book curriculum list");
        }

        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{command.SchoolYear!.Value}, classBookId:{command.ClassBookId!.Value}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsAttendanceAbsenceTopicModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.Date!.Value,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        if (command.StudentPersonIds != null &&
            command.StudentPersonIds.Any() &&
            !await this.TopicsDplrQueryRepository.ExistsTopicStudentsInInstitution(
                command.SchoolYear!.Value,
                command.InstId!.Value,
                command.StudentPersonIds,
                ct))
        {
            throw new DomainValidationException("Students does not belong to the institution.");
        }

        var teacherPersonIds = await this.TopicsDplrQueryRepository.GetCurriculumTeacherIdsAsync(
            command.SchoolYear!.Value,
        command.CurriculumId!.Value,
            ct);

        var start = TimeSpan.Parse(command.StartTime!);
        var end = TimeSpan.Parse(command.EndTime!);

        TopicDplr newTopic =
            new(command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                command.Date!.Value,
                command.Day!.Value,
                command.HourNumber!.Value,
                start,
                end,
                command.CurriculumId!.Value,
                command.Location,
                command.Title!,
                teacherPersonIds,
                command.StudentPersonIds,
                command.SysUserId!.Value);

        await this.TopicAggregateRepository.AddAsync(newTopic, ct);

        await this.UnitOfWork.SaveAsync(ct);

        return newTopic.TopicDplrId;
    }
}
