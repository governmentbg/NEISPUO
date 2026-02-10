namespace MON.Models.Grid
{
    public class AdmissionDocRequestListInput : PagedListInput
    {
        public AdmissionDocRequestListInput()
        {
            SortBy = "IsPermissionGranted asc, CreateDate desc";
        }

        /// <summary>
        /// Идва от грида в UI.
        /// 0 - Изпратени
        /// 1 - Получени
        /// 2 - Приключени
        /// 3 - Всики
        /// </summary>
        public int ListFilter { get; set; } = 3;
    }
}
