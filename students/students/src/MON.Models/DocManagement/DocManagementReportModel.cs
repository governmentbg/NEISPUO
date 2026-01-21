namespace MON.Models.DocManagement
{
    using System.Collections.Generic;

    public class DocManagementReportModel
    {
        public string institutionName { get; set; }
        public string institutionTown { get; set; }
        public string institutionMunicipality { get; set; }
        public string institutionRegion { get; set; }
        public string today { get; set; }
        public string schoolYearName { get; set; }

        /// <summary>
        /// Информация за заявени, получени и използвани документи
        /// </summary>
        public List<DocManagementApplicationReportDocumentModel> applicationDocuments { get; set; } = new List<DocManagementApplicationReportDocumentModel>();

        /// <summary>
        /// Информация за заявени документи от други институции
        /// </summary>
        public List<DocManagementApplicationReportDocumentModel> requestedDocuments { get; set; } = new List<DocManagementApplicationReportDocumentModel>();

        /// <summary>
        /// Информация за прехвърлени документи към други институции
        /// </summary>
        public List<DocManagementApplicationReportDocumentModel> transferredDocuments { get; set; } = new List<DocManagementApplicationReportDocumentModel>();

        /// <summary>
        /// Налични бланки на дубликати от предходни отчетни периоди 
        /// </summary>
        public List<DocManagementApplicationReportDocumentModel> prevPeriodsDocuments { get; set; } = new List<DocManagementApplicationReportDocumentModel>();

        /// <summary>
        /// Бланки на документи предадени за унищожаване, групирани по документ
        /// </summary>
        public List<DocManagementApplicationReportDocumentModel> destructionDocuments { get; set; } = new List<DocManagementApplicationReportDocumentModel>();
    }
}
