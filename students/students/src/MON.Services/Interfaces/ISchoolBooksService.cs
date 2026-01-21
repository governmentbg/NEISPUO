using MON.DataAccess;
using MON.Models;
using MON.Models.StudentModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{

    public interface ISchoolBooksService
    {
        Task<List<SchoolBooksGrade>> GetSchoolBooksGrades(int personId, int schoolYear, int classGroupId);
        Task<List<StudentSanctionModel>> GetSchoolBooksSanctions(int personId, short? schoolYear, int? insitutionId);
        IQueryable<StudentGradeForBasicClassDto> GetStudentGradesForBasicClass(int personId, int basicClassId, int? institutionId, int? schoolYear);
        IQueryable<StudentGradeEvaluationDto> GetStudentGradeEvaluations(int personId, string basicClasses, string curriculumParts);
    }
}