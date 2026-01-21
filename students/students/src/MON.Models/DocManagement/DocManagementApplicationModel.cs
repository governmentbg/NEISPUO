namespace MON.Models.DocManagement
{
    using System;
    using System.Collections.Generic;

    public class DocManagementApplicationModel
    {
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        public int CampaignId { get; set; }
        public int InstitutionId { get; set; }

        /// <summary>
        /// Одобряващо учебно заведение, когато става въпрос за размяна на документи между учебни заведения
        /// </summary>
        public int? ApprovingInstitutionId { get; set; }
        public short SchoolYear { get; set; }
        public string Status { get; set; }
        public string DeliveryNotes { get; set; }
        public IEnumerable<DocumentModel> Attachments { get; set; } = Array.Empty<DocumentModel>();
        public IEnumerable<DocManagementApplicationBasicDocumentModel> BasicDocuments { get; set; } = Array.Empty<DocManagementApplicationBasicDocumentModel>();
    }

    public class DocManagementApplicationBasicDocumentModel
    {
        public int Id { get; set; }
        public int BasicDocumentId { get; set; }
        public string BasicDocumentName { get; set; }
        public int Number { get; set; }
        public int? FreeDocCount { get; set; }
        public bool HasFactoryNumber { get; set; }
        public string DeliveryNotes { get; set; }
        public int? DeliveredCount { get; set; }
        public string DeliveredNumbers { get; set; }
        public bool IsDuplicate { get; set; }
        public string SeriesFormat { get; set; }
        public List<DocManagementApplicationDeliveredDocModel> DeliveredItems { get; set; }
        public short SchoolYear { get; set; }
    }
    public class DocManagementApplicationDeliveredDocModel
    {
        public string DocNumber { get; set; }
        public short? Edition { get; set; }
        public string Series { get; set; }
    }

    public class DocManagementExchangeRequestBasicDocumentModel : DocManagementApplicationBasicDocumentModel
    {
        public new int? Number { get; set; }
    }

}
