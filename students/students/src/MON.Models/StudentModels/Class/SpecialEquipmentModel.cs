namespace MON.Models.StudentModels.Class
{
    public class SpecialEquipmentModel
    {
        public int Id { get; set; }
        public int EquipmentTypeId { get; set; }
        public int StudentClassID { get; set; }
        public bool IsAvailable { get; set; }
        public int PersonId { get; set; }
    }
}
