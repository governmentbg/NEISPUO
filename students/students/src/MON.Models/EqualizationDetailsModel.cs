namespace MON.Models
{
    public class EqualizationDetailsModel
    {
        public int? Id { get; set; }
        public int SubjectId { get; set; }
        public int SortOrder { get; set; }
        public int? BasicClassId { get; set; }
        public string SubjectName { get; set; }
        public string GradeName { get; set; }
        public string BasicClassName { get; set; }

        public int GradeCategory { get; set; }
        public decimal? Grade { get; set; }
        public int? SpecialNeedsGrade { get; set; }
        public int? OtherGrade { get; set; }
        public string Uid { get; set; }
        public int? SubjectTypeId { get; set; }
        public string SubjectTypeName { get; set; }
        public int? Term { get; set; }
        public int? Horarium { get; set; }
        public string SessionName { get; set; }
        public int? SessionId { get; set; }

        public int? GetBasciClassId(int reasonId, int? modelBasicClassId)
        {
            // 1 - преместване на ученика от VIII до XII клас вкл. (чл. 32, ал. 1, т. 1 от Наредба №11
            // 2 - приемане на ученик от обединено училище в XI клас (чл. 32, ал. 1, т. 2 от Наредба №11
            // 3 - лице, прекъснало обучението си (чл. 32, ал. 1, т. 3 от Наредба №11)

            return reasonId switch
            {
                1 => modelBasicClassId,
                2 => 10,
                3 => this.BasicClassId,
                _ => null
            };
        }
    }
}
