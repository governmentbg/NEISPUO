namespace RegStamps.Models.Stamp.EditStamp.Database
{
    public class EditRequestFileDatabaseModel
    {
        public int FileId { get; set; }
        public int RequestId { get; set; }
        public int SchoolId { get; set; }
        public string FileName { get; set; }
        public string FileData { get; set; }
        public int FileType { get; set; }
    }
}
