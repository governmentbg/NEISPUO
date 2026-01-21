namespace RegStamps.Models.Stamp.EditStamp.Database
{
    public class EditRequestDatabaseModel
    {
        public int StampId { get; set; }
        public int RequestId { get; set; }
        public int SchoolId { get; set; }
        public int KeeperId { get; set; }
        public int KeepPlaceId { get; set; }
        public string KeepPlaceName { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; } = null;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
    }
}
