namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateTopicExtApiCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Topic> TopicAggregateRepository,
    ITopicsQueryRepository TopicsQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateTopicExtApiCommand, int>
{
    public async Task<int> Handle(CreateTopicExtApiCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int classBookId = command.ClassBookId!.Value;
        int scheduleLessonId = command.ScheduleLessonId!.Value;
        DateTime date = command.Date!.Value;

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

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsAttendanceAbsenceTopicModificationsAsync(
            schoolYear,
            classBookId,
            date,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        if (!await this.ClassBookCachedQueryStore.ExistsScheduleLessonForClassBookAsync(
                schoolYear,
                classBookId,
                date,
                scheduleLessonId,
                ct))
        {
            throw new DomainValidationException("This scheduleLesson is not in any of the classbook's schedules.");
        }

        var teachers = await this.TopicsQueryRepository.GetScheduleLessonsTeachersAsync(
            schoolYear,
            new [] { scheduleLessonId },
            ct);

        var teacherAbsenceId = await this.ClassBookCachedQueryStore.GetScheduleLessonTeacherAbsenceIdAsync(
            schoolYear,
            scheduleLessonId,
            ct);

        Topic topic = new(
            schoolYear,
            classBookId,
            (command.Titles ?? new[] { command.Title! })
                .Select(t => (title: t, classBookTopicPlanItemId: (int?)null))
                .ToArray(),
            teachers.Select(t => (personId: t.PersonId, isReplTeacher: t.IsReplTeacher)).ToArray(),
            date,
            scheduleLessonId,
            teacherAbsenceId,
            command.SysUserId!.Value);

        await this.TopicAggregateRepository.AddAsync(topic, ct);

        await this.UnitOfWork.SaveAsync(ct);

        return topic.TopicId;
    }
}
