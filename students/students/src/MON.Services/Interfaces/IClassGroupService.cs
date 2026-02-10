namespace MON.Services.Interfaces
{
    using MON.Models;
    using MON.Models.Dashboards;
    using MON.Models.Institution;
    using MON.Models.StudentModels;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IClassGroupService
    {
        Task<ClassGroupViewModel> GetById(int id, CancellationToken cancellationToken);
        Task<List<StudentInfoExternalModel>> GetStudents(int classId, CancellationToken cancellationToken);
        Task<List<StudentForAdmissionModel>> GetStudentsForEnrollment(int classId, CancellationToken cancellationToken);
        Task<List<StudentForAdmissionModel>> GetStudentsForMassEnrollment(int classId, CancellationToken cancellationToken);

        /// <summary>
        /// Връща <see cref="ClassGroupCacheModel"/> кеширан за 1 час
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        Task<ClassGroupCacheModel> GetClassGroupCache(int classId);
    }
}
