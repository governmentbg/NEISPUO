namespace MON.Models
{
    using System.Collections.Generic;

    public class EqualizationModel
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public int ReasonId { get; set; }

        //Ако причината е: преместване на ученика от VIII до XII клас вкл. (чл. 32, ал. 1, т. 1 от Наредба №11 - трябва да запишем и класа
        public int? InClass { get; set; }

        public IEnumerable<EqualizationDetailsModel> EqualizationDetails { get; set; }
        public IEnumerable<DocumentViewModel> Documents { get; set; }

        public short? SchoolYear { get; set; }
        public string SchoolYearName { get; set; }

        public string ReasonName { get; set; }
        public string BasicClassName { get; set; }


    }
}
