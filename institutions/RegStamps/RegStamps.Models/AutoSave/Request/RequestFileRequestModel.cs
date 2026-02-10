namespace RegStamps.Models.AutoSave.Request
{
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;

    public class RequestFileRequestModel
    {
        [Required]
        public int? RequestId { get; set; } = null;

        [Range(0,10)]
        public int FileTypeVal { get; set; }

        [Required]
        public IFormFile? UploadData { get; set; } = null;
    }
}
