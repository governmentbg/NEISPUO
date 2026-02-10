namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateGradeExtApiCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Grade> GradeAggregateRepository,
    INotificationsQueryRepository NotificationsQueryRepository,
    INotificationsService NotificationsService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateGradeExtApiCommand, int>
{
    public async Task<int> Handle(CreateGradeExtApiCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int classBookId = command.ClassBookId!.Value;

        if (command.DecimalGrade == null &&
            command.QualitativeGrade == null &&
            command.SpecialGrade == null)
        {
            throw new DomainValidationException("A grade must be specified");
        }

        if ((command.Category == GradeCategory.Decimal && (command.QualitativeGrade != null || command.SpecialGrade != null)) ||
            (command.Category == GradeCategory.Qualitative && (command.DecimalGrade != null || command.SpecialGrade != null)) ||
            (command.Category == GradeCategory.SpecialNeeds && (command.DecimalGrade != null || command.QualitativeGrade != null)))
        {
            throw new DomainValidationException($"Missmatch between the grade category and the grade specified");
        }

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

        if (command.Category == GradeCategory.Decimal)
        {
            if (!await this.ClassBookCachedQueryStore.CheckBookTypeAllowsDecimalGradesAsync(schoolYear, classBookId, ct))
            {
                throw new DomainValidationException($"Cannot create decimal grade for the book type of classBookId:{classBookId}");
            }
        }

        if (command.Category == GradeCategory.Qualitative)
        {
            if (!await this.ClassBookCachedQueryStore.CheckBookTypeAllowsQualitativeGradesAsync(schoolYear, classBookId, ct))
            {
                throw new DomainValidationException($"Cannot create qualitative grade for the book type of classBookId:{classBookId}");
            }
        }

        if (command.Category == GradeCategory.SpecialNeeds)
        {
            if (!await this.ClassBookCachedQueryStore.CheckBookTypeAllowsSpecialGradesAsync(schoolYear, classBookId, ct))
            {
                throw new DomainValidationException($"Cannot create special needs grade for the book type of classBookId:{classBookId}");
            }
        }

        if (command.Type == GradeType.Final)
        {
            if (await this.ClassBookCachedQueryStore.CheckIsFirstGradeClassBookAsync(schoolYear, classBookId, ct))
            {
                throw new DomainValidationException($"Cannot create final grade for the book type of classBookId:{classBookId}. A FirstGradeResult should be created instead.");
            }
        }

        if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                schoolYear,
                classBookId,
                command.PersonId!.Value,
                ct))
        {
            throw new DomainValidationException("This person is not in the class book students list");
        }

        int curriculumId;
        if (command.ScheduleLessonId != null)
        {
            int? cId = await this.ClassBookCachedQueryStore.GetScheduleLessonCurriculumIdForClassBookAsync(
                schoolYear,
                classBookId,
                command.Date!.Value,
                command.ScheduleLessonId!.Value,
                command.PersonId!.Value,
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
                    curriculumId,
                    ct))
            {
                throw new DomainValidationException($"This curriculum is not in the class book curriculum list");
            }
        }

        if (!await this.ClassBookCachedQueryStore.CheckPersonExistsInCurriculumStudentsOrItsProfilingSubjectAsync(schoolYear, curriculumId, command.PersonId.Value, ct))
        {
            throw new DomainValidationException($"Person with id:{command.PersonId.Value} is not enrolled in curriculum:{curriculumId}");
        }

        SchoolTerm term;
        if (Grade.GradeTypeRequiresScheduleLesson(command.Type!.Value))
        {
            term = await this.ClassBookCachedQueryStore.GetTermForDateAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                command.Date!.Value, ct);
        }
        else if (command.Type == GradeType.Final)
        {
            term = SchoolTerm.TermTwo;
        }
        else
        {
            term = command.Term ??
                await this.ClassBookCachedQueryStore.GetTermForDateAsync(
                    command.SchoolYear!.Value,
                    command.ClassBookId!.Value,
                    command.Date!.Value, ct);
        }

        // TODO check if student:
        //  - has SpecialNeeds and allow only corresponding grade

        Grade? grade = null;

        int? teacherAbsenceId = null;

        if (command.ScheduleLessonId.HasValue)
        {
            teacherAbsenceId = await this.ClassBookCachedQueryStore.GetScheduleLessonTeacherAbsenceIdAsync(
                schoolYear,
                command.ScheduleLessonId!.Value,
                ct);
        }

        if (command.DecimalGrade.HasValue)
        {
            var subjectTypeId = await this.ClassBookCachedQueryStore.GetCurriculumSubjectTypeIdAsync(
                schoolYear,
                curriculumId,
                ct);

            grade = new GradeDecimal(
                schoolYear,
                classBookId,
                command.PersonId!.Value,
                curriculumId,
                subjectTypeId,
                command.DecimalGrade.Value,
                command.Type!.Value,
                term,
                command.Date!.Value,
                command.ScheduleLessonId,
                teacherAbsenceId,
                command.Comment,
                command.SysUserId!.Value);
        }

        if (command.QualitativeGrade.HasValue)
        {
            grade = new GradeQualitative(
                schoolYear,
                classBookId,
                command.PersonId!.Value,
                curriculumId,
                command.QualitativeGrade.Value,
                command.Type!.Value,
                term,
                command.Date!.Value,
                command.ScheduleLessonId,
                teacherAbsenceId,
                command.Comment,
                command.SysUserId!.Value);
        }

        if (command.SpecialGrade.HasValue)
        {
            grade = new GradeSpecialNeeds(
                schoolYear,
                classBookId,
                command.PersonId!.Value,
                curriculumId,
                command.SpecialGrade.Value,
                command.Type!.Value,
                term,
                command.Date!.Value,
                command.ScheduleLessonId,
                teacherAbsenceId,
                command.Comment,
                command.SysUserId!.Value);
        }

        await this.GradeAggregateRepository.AddAsync(
            grade ?? throw new DomainException("Grade should not be null here."),
            ct);

        await this.UnitOfWork.SaveAsync(ct);

        return grade.GradeId;
    }
}
