namespace MON.Services.Interfaces
{
    using MON.Models.Diploma;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBarcodeYearService
    {
        Task AddBarcodeYearAsync(BarcodeYearModel model);
        Task DeleteBarcodeYearAsync(int barcodeYearId);
        Task<BarcodeYearModel> GetBarcodeYearByIdAsync(int barcodeYearId);
        Task<BarcodeYearListViewModel> GetBarcodeYearsAsync(int basicDocumentId);
        Task<IEnumerable<BarcodeYearModel>> GetBarcodesByYear(int basicDocumentId, short schoolYear);
        Task UpdateBarcodeYearAsync(BarcodeYearModel model);
    }
}
