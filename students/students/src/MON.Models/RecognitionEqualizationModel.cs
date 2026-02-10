namespace MON.Models
{
    using MON.Shared.Enums;

    public class RecognitionEqualizationModel
    {
        public int? Id { get; set; }
        public int SubjectId { get; set; }
        public int GradeCategory { get; set; }
        public decimal? Grade { get; set; }
        public int? SpecialNeedsGrade { get; set; }
        public int? OtherGrade { get; set; }
        public int? SortOrder { get; set; }
        public string OriginalSubject { get; set; }
        public string OriginalGrade { get; set; }
        /// <summary>
        /// Оценката е задължителна според образователното ниво и класа на приравняване.
        /// </summary>
        public bool IsRequired { get; set; }
        public string Uid { get; set; }
        public string SubjectName { get; set; }
        public int? SubjectTypeId { get; set; }
        public string SubjectTypeName { get; set; }
        public int? BasicClassId { get; set; }
        public string BasicClassName { get; set; }

        public int? GetBasciClassId(int educationLevelId, int? modelBasicClassId)
        {
            return educationLevelId switch

            {
                (int)RecognitionEduLevelEnum.Primary => 7,
                (int)RecognitionEduLevelEnum.Secondary => 12,
                (int)RecognitionEduLevelEnum.Qualification => this.BasicClassId,
                (int)RecognitionEduLevelEnum.Class => modelBasicClassId,
                _ => null
            };
        }
    }
}
