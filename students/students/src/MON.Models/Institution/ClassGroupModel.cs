namespace MON.Models.Institution
{
    using System.Collections.Generic;

    public class ClassGroupBaseModel
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int? BasicClassId { get; set; }
        public string Name { get; set; }
        public string BasicClassName { get; set; }
        public string BasicClassDescription { get; set; }
        public string ClassTypeName { get; set; }
        public string EduFormName { get; set; }
        public int Count { get; set; }
        public int? SchoolYear { get; set; }
        public int? ParentClassId { get; set; }
        public bool IsValid { get; set; }
        public int? ParentBasicClassId { get; set; }
        public string ParentBasicClassName { get; set; }
        public string Profession { get; set; }
        public string Speciality { get; set; }
        public int? StudentCountPlaces { get; set; }
        public bool IsBasicClass { get; set; }
        public string ParenClassName { get; set; }
        public bool IsClosed { get; set; }   
    }

    public class ClassGroupModel : ClassGroupBaseModel
    {
        public IEnumerable<StudentInfoModel> Students { get; set; }
    }
}
