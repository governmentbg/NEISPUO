namespace RegStamps.Models.AutoSave.Request
{
    using Microsoft.AspNetCore.Http;

    public class StampFileRequestModel
    {
        public int StampId { get; set; }

        public IFormFile UploadData { get; set; }
    }
}
