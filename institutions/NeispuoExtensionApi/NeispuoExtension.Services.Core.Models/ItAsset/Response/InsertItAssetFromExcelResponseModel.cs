namespace NeispuoExtension.Services.Core.Models.ItAsset.Response
{
    using System.Collections.Generic;

    public class InsertItAssetFromExcelResponseModel
    {
        public bool Success { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }
}
