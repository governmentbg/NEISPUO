namespace RegStamps.Models.Stamp.ListStamp.Database
{
    using System.ComponentModel.DataAnnotations;

    public class StampDataDatabaseModel
    {
        [Display(Name = "УИН на печата")]
        public int StampId { get; set; }

        [Display(Name = "Код по НЕИСПУО")]
        public int SchoolId { get; set; }

        [Display(Name = "Тип на печата")]
        public string StampTypeName { get; set; }

        public int StampTypeId { get; set; }

        [Display(Name = "Статус на печата")]
        public string StampStatusName { get; set; }

        public int StampStatusId { get; set; }

        [Display(Name = "Дата на първоначално използване")]
        public DateTime? FirstUseDate { get; set; } = null;

        [Display(Name = "ЕГН на текущият пазител")]
        public string KeeperIdNumber { get; set; }

        [Display(Name = "Имена на текущият пазител")]
        public string KeeperFullName { get; set; }

        public string StampIdByte { get; set; }

        public int RequestStatusId { get; set; }

        public int RequestTypeId { get; set; }

        public int CurrentRequestId { get; set; }
        public string CurrentRequestIdByte { get; set; }
    }
}
