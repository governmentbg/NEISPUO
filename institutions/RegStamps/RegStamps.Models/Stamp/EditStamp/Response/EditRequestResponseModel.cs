namespace RegStamps.Models.Stamp.EditStamp.Response
{
    using System.ComponentModel.DataAnnotations;

    public class EditRequestResponseModel
    {
        [Display(Name = "УИН на печата")]
        public int StampId { get; set; }

        [Display(Name = "Заявка на печата")]
        public int RequestId { get; set; }

        public string RequestIdByte { get; set; }

        [Display(Name = "Код по НЕИСПУО")]
        public int SchoolId { get; set; }

        public int KeeperId { get; set; }

        public int KeepPlaceId { get; set; }

        [Display(Name = "Място на съхранение")]
        public string KeepPlaceName { get; set; }

        [Display(Name = "№ на Заповед")]
        public string OrderNumber { get; set; }

        [Display(Name = "Дата на Заповед")]
        public string OrderDate { get; set; }

        [Display(Name = "Начална дата на връчване")]
        public string StartDate { get; set; }

        [Display(Name = "Крайна дата на съхранение")]
        public string EndDate { get; set; }
    }
}
