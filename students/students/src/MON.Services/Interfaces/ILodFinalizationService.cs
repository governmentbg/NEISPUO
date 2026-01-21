namespace MON.Services.Interfaces
{
    using MON.Models.StudentModels.Lod;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ILodFinalizationService
    {
        Task ApproveLodAsync(LodFinalizationModel model);
        Task ApproveLodUndoAsync(LodFinalizationUndoModel model);
        Task<bool> IsLodFInalized(int personId, short schoolYear, CancellationToken cancellationToken = default);
        Task<bool> IsLodApproved(int personId, short schoolYear, CancellationToken cancellationToken = default);
        Task<List<short>> GetStudentFinalizedLods(int personId);
        Task SignLodAsync(LodSignatureModel model);
        Task SignLodUndoAsync(LodSignatureUndoModel model);
    }
}
