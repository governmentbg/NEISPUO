namespace MON.Models.DocManagement
{
    public class DocManagementReportImputModel
    {
        public int? ApplicationId { get; set; }
        public int? CampaignId { get; set; }
        public int? RegionId { get; set; }
        public int? MunicipalityId { get; set; }
        public short SchoolYear { get; set; }

        /// <summary>
        /// 1 - Всички, 2 - С делегиран бюджет, 3 - Без делегиран бюджет
        /// </summary>
        public short? InstType { get; set; }

        /// <summary>
        /// 1 - Без групиране, 2 - Групиране по регион, 3 - Групиране по община
        /// </summary>
        public short? GroupingType { get; set; }

        /// <summary>
        /// 1 - Всички, 2 - Основни кампании, 3 - Допълнителни кампании
        /// </summary>
        public short? CampaignType { get; set; }
    }
}
