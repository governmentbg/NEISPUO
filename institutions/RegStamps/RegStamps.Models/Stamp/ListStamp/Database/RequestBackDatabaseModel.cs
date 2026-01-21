namespace RegStamps.Models.Stamp.ListStamp.Database
{
    using System;

    public class RequestBackDatabaseModel
    {
        public int RequestId { get; set; }
        public string RequestIdByte { get; set; }
        public int StampId { get; set; }
        public string StampIdByte { get; set; }
        public int KeeperId { get; set; }
        public int PlaceId { get; set; }
        public int RequestStatusId { get; set; }
        public int RequestTypeId { get; set; }
        public string MonNotes { get; set; }
        public DateTime? RequestBackDate { get; set; } = null;
    }
}
