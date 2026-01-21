namespace MON.Models
{
    public class AbsenceReportListInput : PagedListInput
    {
        public AbsenceReportListInput()
        {
            SortBy = "SchoolYear desc, Month desc";
        }

        public bool? OnlyActiveAbsenceCampaigns { get; set; }

        /// <summary>
        /// Идва от грида в UI.
        /// 0 - Неподадени отъствия
        /// 1 - Подадени отсъствия
        /// 2 - Всики
        /// </summary>
        public int ReportTypeFilter { get; set; }

        /// <summary>
        /// Идва от грида в UI.
        /// 0 - Неактивни кампании
        /// 1 - Активни кампании
        /// 2 - Всики
        /// </summary>
        public int CampaignStatusFilter { get; set; }

        public short? SchoolYear { get; set; }

        public short? Month { get; set; }
    }
}
