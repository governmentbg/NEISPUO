namespace SB.Domain;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateTopicsCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Topic> TopicAggregateRepository,
    ITopicsQueryRepository TopicsQueryRepository,
    IClassBookTopicPlanItemsAggregateRepository ClassBookTopicPlanItemsAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateTopicsCommand, int[]>
{
    public async Task<int[]> Handle(CreateTopicsCommand command, CancellationToken ct)
    {
        int[] classBookTopicPlanItemIds = command.Topics!
            .SelectMany(t => t.ClassBookTopicPlanItemIds ?? Enumerable.Empty<int>())
            .Distinct()
            .ToArray();

        Dictionary<int, ClassBookTopicPlanItem> classBookTopicPlanItems;
        if (classBookTopicPlanItemIds.Any())
        {
            classBookTopicPlanItems =
                (await this.ClassBookTopicPlanItemsAggregateRepository.FindAllByIdsAsync(
                    command.SchoolYear!.Value,
                    command.ClassBookId!.Value,
                    classBookTopicPlanItemIds,
                    ct))
                .ToDictionary(tpi => tpi.ClassBookTopicPlanItemId, tpi => tpi);
        }
        else
        {
            classBookTopicPlanItems = new();
        }

        var teachers = await this.TopicsQueryRepository.GetScheduleLessonsTeachersAsync(
            command.SchoolYear!.Value,
            command.Topics!.Select(t => t.ScheduleLessonId!.Value).ToArray(),
            ct);

        var teachersByScheduleLessonId = teachers
            .ToLookup(t => t.ScheduleLessonId, t => (personId: t.PersonId, isReplTeacher: t.IsReplTeacher));

        List<int> createdTopicIds = new();
        foreach (var topic in command.Topics!)
        {
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
                topic.Date!.Value,
                ct))
            {
                throw new DomainValidationException($"The classbook is locked.");
            }

            if (!await this.ClassBookCachedQueryStore.ExistsScheduleLessonForClassBookAsync(
                    command.SchoolYear!.Value,
                    command.ClassBookId!.Value,
                    topic.Date!.Value,
                    topic.ScheduleLessonId!.Value,
                    ct))
            {
                throw new DomainValidationException("This scheduleLesson is not in any of the classbook's schedules.");
            }

            (string title, int? classBookTopicPlanItemId)[] titles;
            if (topic.Title != null)
            {
                titles = new[] { (title: topic.Title, classBookTopicPlanItemId: (int?)null) };
            }
            else if (topic.ClassBookTopicPlanItemIds?.Any() == true)
            {
                titles = new (string title, int? classBookTopicPlanItemId)[topic.ClassBookTopicPlanItemIds.Length];
                for (int i = 0; i < topic.ClassBookTopicPlanItemIds.Length; i++)
                {
                    int classBookTopicPlanItemId = topic.ClassBookTopicPlanItemIds[i];

                    ClassBookTopicPlanItem classBookTopicPlanItem = classBookTopicPlanItems[classBookTopicPlanItemId];
                    if (!classBookTopicPlanItem.Taken)
                    {
                        classBookTopicPlanItem.UpdateTaken(true, command.SysUserId!.Value);
                    }

                    titles[i] = (classBookTopicPlanItem.Title, classBookTopicPlanItemId);
                }
            }
            else
            {
                throw new DomainValidationException("Either Title or ClassBookTopicPlanItemIds must be specified.");
            }

            var teacherAbsenceId = await this.ClassBookCachedQueryStore.GetScheduleLessonTeacherAbsenceIdAsync(
                command.SchoolYear!.Value,
                topic.ScheduleLessonId!.Value,
                ct);

            if (teacherAbsenceId != topic.TeacherAbsenceId)
            {
                throw new DomainValidationException("teacherAbsenceId is invalid.");
            }

            Topic newTopic =
                new(command.SchoolYear!.Value,
                    command.ClassBookId!.Value,
                    titles,
                    teachersByScheduleLessonId[topic.ScheduleLessonId!.Value].ToArray(),
                    topic.Date!.Value,
                    topic.ScheduleLessonId!.Value,
                    topic.TeacherAbsenceId,
                    command.SysUserId!.Value);

            await this.TopicAggregateRepository.AddAsync(newTopic, ct);
            createdTopicIds.Add(newTopic.TopicId);
        }

        await this.UnitOfWork.SaveAsync(ct);

        return createdTopicIds.ToArray();
    }
}
