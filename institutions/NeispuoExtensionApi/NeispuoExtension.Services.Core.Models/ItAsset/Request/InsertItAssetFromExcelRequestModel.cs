namespace NeispuoExtension.Services.Core.Models.ItAsset.Request
{
    using Microsoft.AspNetCore.Http;

    public class InsertItAssetFromExcelRequestModel
    {
        public IFormFile File { get; set; }

        public int InstitutionDepartmentId { get; set; }
    }
}
