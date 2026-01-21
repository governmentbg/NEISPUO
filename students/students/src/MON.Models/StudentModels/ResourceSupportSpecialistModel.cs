namespace MON.Models.StudentModels
{
    public class ResourceSupportSpecialistModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string OrganizationName { get; set; }
        public int ResourceSupportSpecialistTypeId { get; set; }
        public int WorkPlaceId { get; set; }
        public int ResourceSupportId { get; set; }
        public string OrganizationType { get; set; }
        public string SpecialistType { get; set; }
        public int? SysUserID { get; set; }
        public float? WeeklyHours { get; set; }
        public string ResourceSupportSpecialistTypeName { get; set; }
        public string WorkPlaceName { get; set; }
    }
}
