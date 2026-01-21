namespace MON.Models.StudentModels
{
    using MON.Shared;
    using MON.Shared.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LodEvaluationListDataModel
    {
        public int? StudentClassId { get; set; }
        public string SchoolYearName { get; set; }
        public string ClassName { get; set; }
        public string SpecialityName { get; set; }
        public string ProfessionName { get; set; }
        public string EduFormName { get; set; }
        public string BasicClassName { get; set; }
        public string ClassTypeName { get; set; }
        public string EvaluationTypes { get; set; }
        public short? SchoolYear { get; set; }
        public List<string> EvaluationTypeList => (EvaluationTypes ?? "")
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .ToHashSet()
                                .ToList();
        public int? EvaluationTypeId => EvaluationTypeList.IsNullOrEmpty()
            ? null
            : (int?)EvaluationTypeList.FirstOrDefault().TryParseEnum<LodEvaluationType>();
    }
}
