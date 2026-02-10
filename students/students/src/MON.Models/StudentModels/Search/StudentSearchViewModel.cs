using MON.Models.Enums;

namespace MON.Models.StudentModels.Search
{
    public class StudentSearchViewModel : StudentSearchModel
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int Age { get; set; }
        public string FullName { get; set; }
        public int? PositionID { get; set; }
        public int? InstitutionID { get; set; }
        public string Uid { get; set; }
        public bool IsStudent => PositionID.HasValue && (PositionID.Value == (int)PositionType.Student || PositionID.Value == (int)PositionType.StudentSpecialNeeds);
        public bool IsOwner { get; set; }
    }
}
