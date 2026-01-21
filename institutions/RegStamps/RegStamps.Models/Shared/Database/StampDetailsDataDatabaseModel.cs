namespace RegStamps.Models.Shared.Database
{
    using System.ComponentModel.DataAnnotations;

    public class StampDetailsDataDatabaseModel
    {
        [Display(Name = "Уникален индентификационен номер на печата")]
        public int StampId { get; set; }

        [Display(Name = "Код по НЕИСПУО")]
        public int SchoolId { get; set; }

        public int StampTypeId { get; set; }
        public string StampTypeName { get; set; }

        //[Display(Name = "Тип на печата")]
        //public List<DropDownStampTypeViewModel> StampTypeDropDown { get; set; } = new List<DropDownStampTypeViewModel>();

        public int StampStatusId { get; set; }

        public string StampStatusName { get; set; }

        [Display(Name = "Дата на издаване на печата")]
        public DateTime? FirstUseDate { get; set; } = null;

        [Display(Name = "Лице, на което първоначално е предаден")]
        public string FirstUsePerson { get; set; }

        [Display(Name = "Номер на писмото, с което е предаден")]
        public string LetterNumber { get; set; }

        [Display(Name = "Дата на писмото, с което е предаден")]
        public DateTime? LetterDate { get; set; } = null;

        [Display(Name = "Писмо номер и дата на предаване")]
        public string LetterNumberAndDate { get; set; }

        [Display(Name = "Образец на отпечатък")]
        public string Image { get; set; }

        public string ImageName { get; set; }

        public string StampIdByte { get; set; }
    }
}
