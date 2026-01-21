namespace MON.Models.DocManagement
{
    using System.Collections.Generic;

    public class DocManagementApplicationReportModel
    {
        public string schoolYearName { get; set; }
        public string institutionCode { get; set; }
        public string institutionDirectorName { get; set; }
        public string institutionName { get; set; }
        public string institutionPhoneNumber { get; set; }
        public string institutionEmail { get; set; }
        public string institutionAddress { get; set; }
        public string institutionBulstat { get; set; }

        public List<DocManagementApplicationReportDocumentModel> documents { get; set; } = new List<DocManagementApplicationReportDocumentModel>();
    }

    public class DocManagementApplicationReportDocumentModel
    {
        // Ако от даден документ сме получили номера, който не се последователни,
        // то те се групират в отделни групи от последователни номера.


        /// <summary>
        /// Да не се използва в word документи. Само за вътрешни нужди.
        /// </summary>
        public int BasicDocumentId { get; set; }

        /// <summary>
        /// Номенклатурен номер на документ
        /// </summary>
        public string nomenclatureNumber { get; set; }
        /// <summary>
        /// Име на документ
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Получена бройка документи
        /// </summary>
        public string count { get; set; }
        /// <summary>
        /// Серия на документ
        /// </summary>
        public string series { get; set; }
        /// <summary>
        /// От - номер на документ
        /// </summary>
        public string fromNumber { get; set; }
        /// <summary>
        /// До - номер на документ
        /// </summary>
        public string toNumber { get; set; }

        /// <summary>
        /// Заявена бройка документи
        /// </summary>
        public string requestedCount { get; set; }

        /// <summary>
        /// Година на издание
        /// </summary>
        public string edition { get; set; }


        public List<DocManagementApplicationReportDocumentModel> documents { get; set; } = new List<DocManagementApplicationReportDocumentModel>();
    }
}
