namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;

internal record CreateGradeCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Grade> GradeAggregateRepository,
    INotificationsQueryRepository NotificationsQueryRepository,
    INotificationsService NotificationsService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateGradeCommand, int[]>
{
    public async Task<int[]> Handle(CreateGradeCommand command, CancellationToken ct)
    {
        if (!command.Students!.Any())
        {
            return Array.Empty<int>();
        }

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

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsGradeModificationsAsync(
            schoolYear,
            classBookId,
            command.Type!.Value,
            command.Date!.Value,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        if (command.Students!.Any(s => s.DecimalGrade.HasValue))
        {
            if (!await this.ClassBookCachedQueryStore.CheckBookTypeAllowsDecimalGradesAsync(schoolYear, classBookId, ct))
            {
                throw new DomainValidationException($"Cannot create decimal grade for the book type of classBookId:{classBookId}");
            }
        }

        if (command.Students!.Any(s => s.QualitativeGrade.HasValue))
        {
            if (!await this.ClassBookCachedQueryStore.CheckBookTypeAllowsQualitativeGradesAsync(schoolYear, classBookId, ct))
            {
                throw new DomainValidationException($"Cannot create qualitative grade for the book type of classBookId:{classBookId}");
            }
        }

        if (command.Students!.Any(s => s.SpecialGrade.HasValue))
        {
            if (!await this.ClassBookCachedQueryStore.CheckBookTypeAllowsSpecialGradesAsync(schoolYear, classBookId, ct))
            {
                throw new DomainValidationException($"Cannot create special needs grade for the book type of classBookId:{classBookId}");
            }
        }

        int curriculumId;
        if (command.ScheduleLessonId != null)
        {
            var personIds =
                command.Students!.Where(s =>
                    s.DecimalGrade != null ||
                    s.QualitativeGrade != null ||
                    s.SpecialGrade != null)
                .Select(s => s.PersonId!.Value)
                .ToArray();

            int? cId = await this.ClassBookCachedQueryStore.GetScheduleLessonCurriculumIdForClassBookAsync(
                schoolYear,
                classBookId,
                command.Date!.Value,
                command.ScheduleLessonId!.Value,
                personIds.Length == 1 ? personIds[0] : null,
                ct);

            if (cId == null || (command.CurriculumId != null && command.CurriculumId != cId))
            {
                throw new DomainValidationException("This scheduleLesson is not in any of the classbook's schedules.");
            }

            curriculumId = cId.Value;
        }
        else
        {
            curriculumId = command.CurriculumId!.Value;

            if (!await this.ClassBookCachedQueryStore.ExistsCurriculumForClassBookAsync(
                    schoolYear,
                    classBookId,
                    command.CurriculumId!.Value,
                    ct))
            {
                throw new DomainValidationException($"This curriculum is not in the class book curriculum list");
            }
        }

        SchoolTerm term;
        if (Grade.GradeTypeRequiresScheduleLesson(command.Type!.Value))
        {
            term = await this.ClassBookCachedQueryStore.GetTermForDateAsync(
                schoolYear,
                classBookId,
                command.Date!.Value, ct);
        }
        else
        {
            term = command.Term ??
                await this.ClassBookCachedQueryStore.GetTermForDateAsync(
                    schoolYear,
                    classBookId,
                    command.Date!.Value, ct);
        }

        List<int> createdGradeIds = new();
        foreach (var student in command.Students!)
        {
            if (student.DecimalGrade != null ||
                student.QualitativeGrade != null ||
                student.SpecialGrade != null)
            {
                if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                        schoolYear,
                        classBookId,
                        student.PersonId!.Value,
                        ct))
                {
                    throw new DomainValidationException($"This person ({student.PersonId!.Value}) is not in the class book students list");
                }

                if (command.ScheduleLessonId.HasValue)
                {
                    var teacherAbsenceId = await this.ClassBookCachedQueryStore.GetScheduleLessonTeacherAbsenceIdAsync(
                        command.SchoolYear!.Value,
                        command.ScheduleLessonId.Value,
                        ct);

                    if (teacherAbsenceId != command.TeacherAbsenceId)
                    {
                        throw new DomainValidationException("teacherAbsenceId is invalid.");
                    }
                }

                Grade grade = await this.ConstructGrade(
                    command.SchoolYear!.Value,
                    command.ClassBookId!.Value,
                    command.SysUserId!.Value,
                    curriculumId,
                    command.Type!.Value,
                    term,
                    command.Date!.Value,
                    command.ScheduleLessonId,
                    command.TeacherAbsenceId,
                    student.Comment,
                    student,
                    ct);
                await this.GradeAggregateRepository.AddAsync(grade, ct);
                createdGradeIds.Add(grade.GradeId);

                var curriculumName = await this.NotificationsQueryRepository.GetCurriculumNameAsync(schoolYear, curriculumId, ct);
                var jObject = JObject.FromObject(new
                {
                    gradeText = grade.GradeText,
                    comment = grade.Comment,
                    curriculumName,
                    date = grade.Date,
                    emailTag = grade.EmailTag,
                    pushNotificationTag = grade.PushNotificationTag
                });

                await this.NotificationsService.TryPostNotificationsAsync("NewGrade", grade.PersonId, jObject, ct);
            }
        }

        await this.UnitOfWork.SaveAsync(ct);

        return createdGradeIds.ToArray();
    }

    private async Task<Grade> ConstructGrade(
        int schoolYear,
        int classBookId,
        int sysUserId,
        int curriculumId,
        GradeType type,
        SchoolTerm term,
        DateTime date,
        int? scheduleLessonId,
        int? teacherAbsenceId,
        string? comment,
        CreateGradeCommandStudent student,
        CancellationToken ct)
    {
        // TODO check if student:
        //  - has SpecialNeeds and allow only corresponding grade

        if (student.DecimalGrade.HasValue)
        {
            var subjectTypeId = await this.ClassBookCachedQueryStore.GetCurriculumSubjectTypeIdAsync(
                schoolYear,
                curriculumId,
                ct);

            return new GradeDecimal(
                schoolYear,
                classBookId,
                student.PersonId!.Value,
                curriculumId,
                subjectTypeId,
                student.DecimalGrade.Value,
                type,
                term,
                date,
                scheduleLessonId,
                teacherAbsenceId,
                comment,
                sysUserId);
        }

        if (student.QualitativeGrade.HasValue)
        {
            return new GradeQualitative(
                schoolYear,
                classBookId,
                student.PersonId!.Value,
                curriculumId,
                student.QualitativeGrade.Value,
                type,
                term,
                date,
                scheduleLessonId,
                teacherAbsenceId,
                comment,
                sysUserId);
        }

        if (student.SpecialGrade.HasValue)
        {
            return new GradeSpecialNeeds(
                schoolYear,
                classBookId,
                student.PersonId!.Value,
                curriculumId,
                student.SpecialGrade.Value,
                type,
                term,
                date,
                scheduleLessonId,
                teacherAbsenceId,
                comment,
                sysUserId);
        }

        throw new Exception("Unknown grade.");
    }
}
