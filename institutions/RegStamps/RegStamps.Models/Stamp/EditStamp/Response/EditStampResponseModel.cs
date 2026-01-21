namespace RegStamps.Models.Stamp.EditStamp.Response
{
    using System.ComponentModel.DataAnnotations;

    using Shared.Database;

    public class EditStampResponseModel
    {
        [Display(Name = "Уникален индентификационен номер на печата")]
        public int StampId { get; set; }
        public string StampIdByte { get; set; }

        [Display(Name = "Код по НЕИСПУО")]
        public int SchoolId { get; set; }

        public int StampTypeId { get; set; }

        [Display(Name = "Дата на издаване на печата")]
        public string FirstUseDate { get; set; }

        [Display(Name = "Лице, на което първоначално е предаден")]
        public string FirstUsePerson { get; set; }

        [Display(Name = "Номер на писмото, с което е предаден")]
        public string LetterNumber { get; set; }

        [Display(Name = "Дата на писмото, с което е предаден")]
        public string LetterDate { get; set; }

        [Display(Name = "Образец на отпечатък")]
        public string Image { get; set; }

        [Display(Name = "Тип на печата")]
        public IEnumerable<StampTypeDatabaseModel> StampTypeDropDown { get; set; } = new List<StampTypeDatabaseModel>();
    }
}
