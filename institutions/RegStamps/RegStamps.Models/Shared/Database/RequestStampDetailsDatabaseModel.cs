namespace RegStamps.Models.Shared.Database
{
    using System.ComponentModel.DataAnnotations;

    public class RequestStampDetailsDatabaseModel
    {
        [Display(Name = "УИН на печата")]
        public int StampId { get; set; }

        [Display(Name = "Заявка на печата")]
        public int RequestId { get; set; }

        [Display(Name = "Код по НЕИСПУО")]
        public int SchoolId { get; set; }

        public int KeeperId { get; set; }

        public int KeepPlaceId { get; set; }

        [Display(Name = "Място на съхранение")]
        public string KeepPlaceName { get; set; }

        [Display(Name = "№ на Заповед")]
        public string OrderNumber { get; set; }

        [Display(Name = "Дата на Заповед")]
        public DateTime? OrderDate { get; set; } = null;

        [Display(Name = "Заповед номер и дата")]
        public string OrderNumberAndDate { get; set; }

        [Display(Name = "Начална дата на връчване")]
        public DateTime? StartDate { get; set; } = null;

        [Display(Name = "Крайна дата на съхранение")]
        public DateTime? EndDate { get; set; } = null;

        public string IsActive { get; set; }

        public string RequestIdByte { get; set; }

        public int RequestStatusId { get; set; }

        public int RequestTypeId { get; set; }

        public string RequestTypeName { get; set; }
    }
}
