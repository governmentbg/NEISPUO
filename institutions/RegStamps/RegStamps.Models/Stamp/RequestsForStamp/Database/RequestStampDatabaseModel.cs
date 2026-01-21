namespace RegStamps.Models.Stamp.RequestsForStamp.Database
{
    using System.ComponentModel.DataAnnotations;

    public class RequestStampDatabaseModel
    {
        [Display(Name = "Заявка номер")]
        public int RequestId { get; set; }

        [Display(Name = "Тип на заявката")]
        public string RequestTypeName { get; set; }

        [Display(Name = "Статус на заявката")]
        public string RequestStatusName { get; set; }

        [Display(Name = "УИН на печата")]
        public int StampId { get; set; }

        public int StampStatusId { get; set; }

        [Display(Name = "Код по НЕИСПУО")]
        public int SchoolId { get; set; }

        [Display(Name = "ЕГН на пазителя")]
        public string KeeperIdNumber { get; set; }

        [Display(Name = "Имена на пазителя")]
        public string KeeperFullName { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Последна промяна")]
        public DateTime? TimeStamp { get; set; } = null;

        public int RequestStatusId { get; set; }

        public int RequestTypeId { get; set; }

        public string StampIdByte { get; set; }

        public string RequestIdByte { get; set; }
    }
}
