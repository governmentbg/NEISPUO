namespace MON.Models.ExternalEvaluation
{
    using System.Collections.Generic;

    public class ExternalEvaluationModel
    {
        public int? Id { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public int PersonId { get; set; }
        public short? SchoolYear { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public string Uid { get; set; }
        public List<ExternalEvaluationItemModel> Evaluations { get; set; }
    }

    public class ExternalEvaluationItemModel
    {
        public int? Id { get; set; }
        public int? SubjectId { get; set; }
        public int? SubjectTypeId { get; set; }
        public string Subject { get; set; }
        public decimal? Grade { get; set; }
        public decimal? OriginalPoints { get; set; }
        public decimal? Points { get; set; }
        public string Description { get; set; }
        public string FLLevel { get; set; }
        public string SubjectTypeName { get; set; }
    }
}
