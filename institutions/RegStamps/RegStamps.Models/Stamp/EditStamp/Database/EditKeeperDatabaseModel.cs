namespace RegStamps.Models.Stamp.EditStamp.Database
{
    public class EditKeeperDatabaseModel
    {
        public int KeeperId { get; set; }
        public int IdType { get; set; }
        public string IdNumber { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        public int OccupationId { get; set; }
    }
}
