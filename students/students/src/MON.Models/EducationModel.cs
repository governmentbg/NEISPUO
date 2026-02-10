namespace MON.Models
{
    public class EducationModel
    {
        public int? BasicClassId { get; set; }
        public string TrainingType { get; set; }
        public string EducationForm { get; set; }
        public string Specialty { get; set; }
        public int RecordedInBookSubjectsPage { get; set; }
        public int RecordedInBookSubjectsNumber { get; set; }
        public int? Group { get; set; }
        public string Grade { get; set; }
        public string ReceptionAfter { get; set; }
        public int SchoolYear { get; set; }
    }

    public class EducationViewModel : EducationModel
    {
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public string School { get; set; }
        public int? NumberInClass { get; set; }
        public int PositionId { get; set; }

        /// <summary>
        /// Клас в институция като на логнатия потребител
        /// </summary>
        public bool IsOwn { get; set; }
    }
}
