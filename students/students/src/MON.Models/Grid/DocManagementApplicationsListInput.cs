namespace MON.Models.Grid
{
    public class DocManagementApplicationsListInput : PagedListInput
    {
        public int? InstitutionId { get; set; }
        public short? SchoolYear { get; set; }
        public int? CampaignId { get; set; }
        public int? TownId { get; set; }
        public int? RegionId { get; set; }
        public int? MunicipalityId { get; set; }
        /// <summary>
        /// 1 - Всички, 2 - С делегиран бюджет, 3 - Без делегиран бюджет
        /// </summary>
        public short? InstType { get; set; } = 1;

        /// <summary>
        /// 1 - Всички, 2 - Основни кампании, 3 - Допълнителни кампании
        /// </summary>
        public short? CampaignType { get; set; }

        public int? BasicDocumentId { get; set; }

        public int? RequestedInstitutionId { get; set; }

        /// <summary>
        /// Идва от грида в UI.
        /// 0 - Неподадени
        /// 1 - Подадени
        /// 2 - Всики
        /// </summary>
        public int ApplicationStatusFilter { get; set; }

        /// <summary>
        /// Идва от грида в UI.
        /// 0 - Неактивни кампании
        /// 1 - Активни кампании
        /// 2 - Всики
        /// </summary>
        public int CampaignStatusFilter { get; set; }

        public string Status { get; set; }
    }
}
