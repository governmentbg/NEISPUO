namespace MON.Models.StudentModels.Lod
{
    using MON.Shared;
    using MON.Shared.Extensions;
    using System.Collections.Generic;
    using System.Linq;

    public class StudentLodAssessmentListModel : LodAssessmentModel
    {
        public string FullName { get; set; }
        public string BasicClassName { get; set; }
        public string SchooYearName { get; set; }
        public int GradesCount { get; set; }
        public string Classes { get; set; }
        public string Categories { get; set; }

        public HashSet<int> UniqueClasses => Classes.IsNullOrWhiteSpace()
            ? null
            : Classes.Split(",", System.StringSplitOptions.RemoveEmptyEntries).ToHashSet<int>();
        public HashSet<string> UniqueCategories => Categories.IsNullOrWhiteSpace()
            ? null
            : Categories.Split(",", System.StringSplitOptions.RemoveEmptyEntries).ToHashSet();

        public bool IsSelfEduForm { get; set; }
        public int InstitutionId { get; set; }
        public int LodAssessmentsCount { get; set; }
        public string BasicClassDescription { get; set; }
        public bool HasLodSource => UniqueCategories != null && UniqueCategories.Any(x => x.Equals("ЛОД", System.StringComparison.OrdinalIgnoreCase));
    }
}
