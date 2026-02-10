namespace MON.Models.StudentModels.PersonalDevelopmentSupport
{
    using MON.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AdditionalPersonalDevelopmentSupportModel
    {
        public int? Id { get; set; }
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public short? FinalSchoolYear { get; set; }
        public int PeriodTypeId { get; set; } = (int)PersonalDevelopmentSupportPeriodTypeEnum.ShortTerm;
        public int? StudentTypeId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsSuspended { get; set; }
        public DateTime? SuspensionDate { get; set; }
        public string SuspensionReason { get; set; }
        public IEnumerable<AdditionalPersonalDevelopmentSupportItemModel> Items { get; set; } = new List<AdditionalPersonalDevelopmentSupportItemModel>();
        public IEnumerable<DocumentModel> Orders { get; set; } = Array.Empty<DocumentModel>();
        public IEnumerable<DocumentModel> Scorecards { get; set; } = Array.Empty<DocumentModel>();
        public IEnumerable<DocumentModel> Plans { get; set; } = Array.Empty<DocumentModel>();
        public IEnumerable<DocumentModel> Documents { get; set; } = Array.Empty<DocumentModel>();
        public IEnumerable<SopDetailsModel> Sop { get; set; } = Array.Empty<SopDetailsModel>();
        public IEnumerable<AdditionalPersonalDevelopmentSupportDocModel> AllDocuments
        {
            get
            {
                return Orders.Select(x => new AdditionalPersonalDevelopmentSupportDocModel
                {
                    Type = nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Order),
                    Document = x
                })
                .Concat(Scorecards.Select(x => new AdditionalPersonalDevelopmentSupportDocModel
                {
                    Type = nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Scorecard),
                    Document = x
                }))
                .Concat(Plans.Select(x => new AdditionalPersonalDevelopmentSupportDocModel
                {
                    Type = nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Plan),
                    Document = x
                }))
                .Concat(Documents.Select(x => new AdditionalPersonalDevelopmentSupportDocModel
                {
                    Type = nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Other),
                    Document = x
                }));
            }
        }
    }

    public class AdditionalPersonalDevelopmentSupportDocModel
    {
        public string Type { get; set; }
        public DocumentModel Document { get; set; }
    }
}
