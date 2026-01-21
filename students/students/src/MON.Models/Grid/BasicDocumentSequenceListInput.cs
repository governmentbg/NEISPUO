namespace MON.Models.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class BasicDocumentSequenceListInput : PagedListInput
    {
        public short? Year { get; set; }
        public string BasicDocuments { get; set; }
        public int? InstitutionId { get; set; }
        public string InstitutionIdFilter { get; set; }
        public string InstitutionIdFilterOp { get; set; }
        public string FullNameFilter { get; set; }
        public string FullNameFilterOp { get; set; }

        /// <summary>
        /// За РУО и РУО експерт има опция за филтриране на рег.номера
        /// 1: Рег.№ на институция
        /// 2: Рег.№ на РУО
        /// </summary>
        public short? RegNumType { get; set; }


        public BasicDocumentSequenceListInput()
        {
            SortBy = "Id desc";
        }
    }
}
