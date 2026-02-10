namespace MON.Models.Diploma
{
    using MON.Shared.Extensions;
    using System;
    using System.Collections.Generic;

    public class BasicDocumentTemplateModel
    {
        public int? Id { get; set; }
        public int BasicDocumentId { get; set; }
        public int? InstitutionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Principal { get; set; }
        public string Deputy { get; set; }
        public short SchoolYear { get; set; }
        public string InstitutionName { get; set; }
        public string BasicDocumentName { get; set; }

        public List<BasicDocumentTemplatePartModel> Parts { get; set; }
        public string DynamicContent { get; set; }
        public bool IsValidation { get; set; }
        /// <summary>
        /// Json с описание на схемата на BasicDocument-а.
        /// </summary>
        public string Schema { get; set; }
        public string BasicDocumentCodeClassName { get; set; }
        public List<CommissionMemberModel> CommissionMembers { get; set; }
        public string CommissionOrderNumber { get; set; }
        public DateTime? CommissionOrderData { get; set; }
        /// <summary>
        /// Позволени випуски, описани в колона BasicClasses в document.BasicDocument
        /// </summary>
        public List<int> BasicClassIds { get; set; }
        /// <summary>
        /// Избран випуск в шаблон на документ
        /// </summary>
        public int? BasicClassId { get; set; }
        public int? RuoRegId { get; set; }
        public string MainBasicDocumentsStr { get; set; }
        public IEnumerable<int> MainBasicDocuments
        {
            get
            {
                string[] splitStr = (MainBasicDocumentsStr ?? "").Split("|", StringSplitOptions.RemoveEmptyEntries);
                HashSet<int> ids = splitStr.ToHashSet<int>();

                return ids;
            }
        }

        public string DetailedSchoolTypesStr { get; set; }

        public IEnumerable<int> DetailedSchoolTypes
        {
            get
            {
                string[] splitStr = (DetailedSchoolTypesStr ?? "").Split("|", StringSplitOptions.RemoveEmptyEntries);
                HashSet<int> ids = splitStr.ToHashSet<int>();

                return ids;
            }
        }
    }
}
