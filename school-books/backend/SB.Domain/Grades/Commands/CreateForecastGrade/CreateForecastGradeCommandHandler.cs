namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;

internal record CreateForecastGradeCommandHandler(
    IUnitOfWork UnitOfWork,
    IStudentSettingsAggregateRepository StudentSettingsAggregateRepository,
    INotificationsQueryRepository NotificationsQueryRepository,
    INotificationsService NotificationsService,
    IScopedAggregateRepository<Grade> GradeAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateForecastGradeCommand, int>
{
    public async Task<int> Handle(CreateForecastGradeCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int classBookId = command.ClassBookId!.Value;

        if (command.DecimalGrade == null &&
            command.QualitativeGrade == null)
        {
            throw new DomainValidationException("A grade must be specified");
        }

        if ((command.Category == GradeCategory.Decimal && command.QualitativeGrade != null) ||
            (command.Category == GradeCategory.Qualitative && command.DecimalGrade != null))
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

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
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

        Grade grade = await this.ConstructForecastGrade(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.SysUserId!.Value,
            command.PersonId!.Value,
            command.CurriculumId!.Value,
            command.Category!.Value,
            command.Type!.Value,
            command.Term!.Value,
            command.DecimalGrade,
            command.QualitativeGrade,
            ct);
        await this.GradeAggregateRepository.AddAsync(grade, ct);

        var curriculumName = await this.NotificationsQueryRepository.GetCurriculumNameAsync(schoolYear, grade.CurriculumId, ct);
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

        await this.UnitOfWork.SaveAsync(ct);

        return grade.GradeId;
    }

    private async Task<Grade> ConstructForecastGrade(
        int schoolYear,
        int classBookId,
        int sysUserId,
        int personId,
        int curriculumId,
        GradeCategory category,
        GradeType type,
        SchoolTerm term,
        decimal? decimalGrade,
        QualitativeGrade? qualitativeGrade,
        CancellationToken ct)
    {
        if (category == GradeCategory.Decimal && decimalGrade.HasValue)
        {
            var subjectTypeId = await this.ClassBookCachedQueryStore.GetCurriculumSubjectTypeIdAsync(
                schoolYear,
                curriculumId,
                ct);

            return new GradeDecimal(
                schoolYear,
                classBookId,
                personId,
                curriculumId,
                subjectTypeId,
                decimalGrade.Value,
                type,
                term,
                DateTime.Now,
                null,
                null,
                null,
                sysUserId);
        }

        if (category == GradeCategory.Qualitative && qualitativeGrade.HasValue)
        {
            return new GradeQualitative(
                schoolYear,
                classBookId,
                personId,
                curriculumId,
                qualitativeGrade.Value,
                type,
                term,
                DateTime.Now,
                null,
                null,
                null,
                sysUserId);
        }

        throw new Exception("Unsupported forecast grade.");
    }
}
