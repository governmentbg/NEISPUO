namespace RegStamps.Models.Shared.Database
{
    using System.ComponentModel.DataAnnotations;

    public class RequestFileDataDatabaseModel
    {
        public int FileId { get; set; }

        [Display(Name = "Заявка номер")]
        public int RequestId { get; set; }

        [Display(Name = "Код по НЕИСПУО")]
        public int SchoolId { get; set; }

        [Display(Name = "Име на документа")]
        public string FileName { get; set; }

        public string FileData { get; set; }

        [Display(Name = "Тип на документа")]
        public string FileType { get; set; }

        [Display(Name = "Прикачи документ")]
        [Required, FileExtensions(Extensions = ".pdf", ErrorMessage = "Incorrect file format")]
        public string UploadData { get; set; }

        public string FileIdByte { get; set; }
    }
}
