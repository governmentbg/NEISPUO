namespace MON.Services.Interfaces
{
    using MON.DataAccess;
    using MON.Models.Grid;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.Lod;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IStudentLODService
    {
        Task<IPagedList<VStudentGeneralTrainingDatum>> GetGeneralTrainingDataList(StudentGeneralTrainingDataListInput input);
        Task<StudentGeneralTrainingDataDetails> GetGeneralTrainingDataDetails(int studentId, int classId);
        Task<List<VCurriculumStudentDetail>> GetCurriculumDetailsByStudentClass(int studentClassId);
        Task<byte[]> GeneratePersonalFile(LodGeneratorModel model, CancellationToken cancellationToken);
        /// <summary>
        /// Метод без автентикация
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="schoolYears"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<byte[]> GeneratePersonalFileInternal(LodGeneratorModel model, CancellationToken cancellationToken);
        Task<byte[]> GeneratePersonalFileForStayAsync(LodGeneratorModel model, CancellationToken cancellationToken);
        Task<IPagedList<LodFinalizationViewModel>> ListFinalizations(LodFinalizationListInput input, CancellationToken cancellationToken);
    }
}
