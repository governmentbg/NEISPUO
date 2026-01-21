namespace MON.Models
{
    using MON.Shared.Enums;
    using MON.Shared.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BasicDocumentDetailsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasBarcode { get; set; }
        public bool HasFactoryNumber { get; set; }
        public bool IsUniqueForStudent { get; set; }
        public bool IsIncludedInRegister { get; set; }
        public bool? HasSubjects { get; set; }
        public List<BasicDocumentPartDetailsModel> DocumetPartsDetails { get; set; }
        public int? AttachedImagesCountMin { get; set; }
        public int? AttachedImagesCountMax { get; set; }
        public bool IsAppendix { get; set; }
        public bool IsDuplicate { get; set; }
        public string SeriesFormat { get; set; }
        public PageOrientationEnum PageOrientation { get; set; }
        public string MainBasicDocuments { get; set; }
        public string DetailedSchoolTypes { get; set; }

        public IEnumerable<int> MainBasicDocumentsList
        {
            get
            {
                string[] splitStr = (MainBasicDocuments ?? "").Split("|", StringSplitOptions.RemoveEmptyEntries);
                HashSet<int> ids = splitStr.ToHashSet<int>();

                return ids;
            }
        }

        public IEnumerable<int> DetailedSchoolTypesList
        {
            get
            {
                string[] splitStr = (DetailedSchoolTypes ?? "").Split("|", StringSplitOptions.RemoveEmptyEntries);
                HashSet<int> ids = splitStr.ToHashSet<int>();

                return ids;
            }
        }
    }

    public class BasicDocumentPartDetailsModel
    {
        public int Id { get; set; }
        public int BasicDocumentId { get; set; }
        public string SubjectTypesStr { get; set; }
        public List<int> SubjectTypes => (SubjectTypesStr ?? "")
                                .Split("|", StringSplitOptions.RemoveEmptyEntries)
                                .ToHashSet<int>()
                                .ToList();
        public string ExternalEvaluationTypesStr { get; set; }
        public List<int> ExternalEvaluationTypes => (ExternalEvaluationTypesStr ?? "")
                                .Split("|", StringSplitOptions.RemoveEmptyEntries)
                                .ToHashSet<int>()
                                .ToList();
    }
}
