namespace RegStamps.Models.Stamp.EditStamp.Response
{
    using System.ComponentModel.DataAnnotations;

    using Models.Shared.Database;

    public class EditRequestFileResponseModel
    {
        //public int FileId { get; set; }

        //public string FileIdByte { get; set; }

        //[Display(Name = "Заявка номер")]
        //public int RequestId { get; set; }

        //[Display(Name = "Код по НЕИСПУО")]
        //public int SchoolId { get; set; }

        //[Display(Name = "Име на документа")]
        //public string FileName { get; set; }

        //public string FileData { get; set; }

        //[Display(Name = "Тип на документа")]
        //public int FileTypeId { get; set; }

        [Display(Name = "Прикачи документ")]
        [Required, FileExtensions(Extensions = ".pdf", ErrorMessage = "Incorrect file format")]
        public string UploadData { get; set; }

        [Display(Name = "Тип на документа")]
        public IEnumerable<FileTypeDatabaseModel> FileTypeDropDown { get; set; } = new List<FileTypeDatabaseModel>();
    }
}
