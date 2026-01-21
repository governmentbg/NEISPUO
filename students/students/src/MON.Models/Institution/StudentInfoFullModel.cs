namespace MON.Models.Institution
{
    using System;

    public class StudentInfoFullModel : StudentInfoModel
    {
        public string Pin { get; set; }
        public string PinType { get; set; }
        public string Position { get; set; }
        public int BasicClassId { get; set; }
        public int? ParentBasicClassId { get; set; }
        public string ParentBasicClassName { get; set; }
        public string ClassTypeName { get; set; }
        public string Profession { get; set; }
        public string Speciality { get; set; }
        public int ResourceSupportStatus { get; set; }
        public object BasicClassName { get; set; }
    }
}
