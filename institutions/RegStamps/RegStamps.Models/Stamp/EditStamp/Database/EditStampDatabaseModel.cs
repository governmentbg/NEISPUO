namespace RegStamps.Models.Stamp.EditStamp.Database
{
    using System;

    public class EditStampDatabaseModel
    {
        public int StampId { get; set; }
        public int SchoolId { get; set; }
        public int StampTypeId { get; set; }
        public DateTime? FirstUseDate { get; set; } = null;
        public string FirstUsePerson { get; set; }
        public string LetterNumber { get; set; }
        public DateTime? LetterDate { get; set; } = null;
        public string Image { get; set; }
    }
}
