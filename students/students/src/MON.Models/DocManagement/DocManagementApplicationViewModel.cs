namespace MON.Models.DocManagement
{
    using MON.Shared.Enums;
    using MON.Shared.Extensions;
    using System;
    using System.Collections.Generic;

    public class DocManagementApplicationViewModel : DocManagementApplicationModel
    {
        public DocManagementCampaignViewModel Campaign { get; set; }
        public DocManagementCampaignViewModel ParentCampaign { get; set; }
        public string InstitutionName { get; set; }
        public string ApprovingInstitutionName { get; set; }
        public string SchoolYearName { get; set; }
        public string StatusName => Status.TryParseEnum<ApplicationStatusEnum>().GetEnumDescription();
        public DateTime CreateDate { get; set; }
        public string Creator { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Updater { get; set; }
        public int? CurrentUserInstitutionCode { get; set; }
        public bool IsEditable => InstitutionId == CurrentUserInstitutionCode
            && (((Campaign?.IsActive ?? false) && !Campaign.IsHidden && Status != nameof(ApplicationStatusEnum.Approved) && Status != nameof(ApplicationStatusEnum.Rejected)) || ((Campaign?.IsHidden ?? false) && Status == nameof(ApplicationStatusEnum.Submitted)) || Status == nameof(ApplicationStatusEnum.ReturnedForCorrection));
        public bool IsReportable {  get; set; }
        public bool IsDeletable => IsEditable && Status != nameof(ApplicationStatusEnum.ReturnedForCorrection) && Status != nameof(ApplicationStatusEnum.Approved)
                && Status != nameof(ApplicationStatusEnum.Rejected);

        public bool CanBeSubmited => InstitutionId == CurrentUserInstitutionCode
            && Status == nameof(ApplicationStatusEnum.Draft) && (Campaign?.IsActive ?? false);
        public bool HasApprovePermission { get; set; }
        public new IEnumerable<DocumentModel> Attachments { get; set; } = Array.Empty<DocumentModel>();
        public int? InstitutionMunicipalityId { get; set; }
        public int? InstitutionRegionId { get; set; }
        public string InstitutionMunicipalityName { get; set; }
        public string InstitutionMunicipalityCode { get; set; }
        public string InstitutionRegionName { get; set; }
        public string InstitutionRegionCode { get; set; }
        public bool HasEvaluationPermission { get; set; }
        public bool IsExchangeRequest { get; set; }
        public int? RequestedInstitutionId { get; set; }
        public string RequestedInstitutionName { get; set; }
    }

    public class DocManagementExchangeRequestModel : DocManagementApplicationModel
    {
        public new IEnumerable<DocManagementExchangeRequestBasicDocumentModel> BasicDocuments { get; set; } = Array.Empty<DocManagementExchangeRequestBasicDocumentModel>();
    }
}
