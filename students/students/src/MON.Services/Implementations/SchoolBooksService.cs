using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;
using MON.DataAccess;
using MON.Models;
using MON.Models.Absence;
using MON.Models.StudentModels;
using MON.Services.Interfaces;
using MON.Shared.Enums.SchoolBooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Implementations
{
    public class SchoolBooksService : BaseService<SchoolBooksService>, ISchoolBooksService
    {
        public SchoolBooksService(DbServiceDependencies<SchoolBooksService> dependencies)
            : base(dependencies)
        {
        }

        //public IEnumerable<VSchoolBooksGrade> GetVSchoolBooksGrades(int personId, int schoolYear, int? studentClassId)
        //{
        //    return _context.VSchoolBooksGrades
        //        .AsNoTracking()
        //        .Where(x => x.StudentId == personId && x.SchoolYear == schoolYear && (!studentClassId.HasValue || x.StudentClassId == studentClassId));
        //}

        public async Task<List<SchoolBooksGrade>> GetSchoolBooksGrades(int personId, int schoolYear, int classGroupId)
        {
            return await _context.SchoolBooksGrades
                .Include(x => x.Curriculum)
                .AsNoTracking()
                .Where(x => x.PersonId == personId && x.SchoolYear == schoolYear && x.ClassBook.ClassId == classGroupId &&
                    (x.Type == (int)GradeTypeEnum.Term || x.Type == (int)GradeTypeEnum.Final))
                .ToListAsync();
        }

        public Task<List<StudentSanctionModel>> GetSchoolBooksSanctions(int personId, short? schoolYear, int? insitutionId)
        {
            IQueryable<VSchoolBooksSanction> query = _context.VSchoolBooksSanctions
                .Where(x => x.PersonId == personId);

            if (schoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == schoolYear.Value);
            }

            if (insitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == insitutionId.Value);
            }

            return query
                .Select(x => new StudentSanctionModel
                {
                    CancelOrderDate = x.CancelOrderDate,
                    CancelOrderNumber = x.CancelOrderNumber,
                    CancelReason = x.CancelReason,
                    Description = x.Description,
                    EndDate = x.EndDate,
                    OrderDate = x.OrderDate,
                    OrderNumber = x.OrderNumber,
                    PersonId = x.PersonId,
                    RuoOrderDate = x.RuoOrderDate,
                    RuoOrderNumber = x.RuoOrderNumber,
                    StartDate = x.StartDate,
                    SanctionTypeId = x.SanctionTypeId,
                    SchoolYear = x.SchoolYear,
                    InstitutionId = x.InstitutionId
                })
             .ToListAsync();
        }

        public IQueryable<StudentGradeForBasicClassDto> GetStudentGradesForBasicClass(int personId, int basicClassId, int? institutionId, int? schoolYear)
        {
            FormattableString queryString = $"SELECT PersonId, InstitutionId, BasicClassId, SchoolYear, CurriculumPartId, CurriculumPart, CurriculumPartName, CurriculumId, SubjectId, SubjectTypeId, SubjectName, BasicSubjectName, BasicSubjectAbrev, FirstTermDecimalGrades, SecondTermDecimalGrades, FirstTermQualitativeGrades, SecondTermQualitativeGrades, FirstTermSpecialGrades, SecondTermSpecialGrades from [student].[fn_student_grades_for_basicClass]({personId}, {basicClassId}, {institutionId}, {schoolYear})";
            IQueryable<StudentGradeForBasicClassDto> query = _context.Set<StudentGradeForBasicClassDto>()
                .FromSqlInterpolated(queryString);

            return query;
        }

        public IQueryable<StudentGradeEvaluationDto> GetStudentGradeEvaluations(int personId, string basicClasses, string curriculumParts)
        {
            FormattableString queryString = $"SELECT PersonId, CurriculumPartId, CurriculumPart, CurriculumPartName, SubjectId, SubjectName, BasicSubjectName, BasicSubjectAbrev, BasicClassId, IsValidForBasicClass, InstitutionId, SchoolYear, GradeFirstTerm, GradeSecondTerm, FinalGrade from [student].[fn_student_grade_evaluations]({personId}, {basicClasses}, {curriculumParts})";
            IQueryable<StudentGradeEvaluationDto> query = _context.StudentGradeEvaluations
                .FromSqlInterpolated(queryString);

            return query;
        }
    }
}
