namespace RegStamps.Models.Shared.Response
{
    using System.ComponentModel.DataAnnotations;

    public class SchoolDataResponseModel
    {
        [Display(Name = "Код по НЕИСПУО")]
        public int SchoolId { get; set; }

        [Display(Name = "Наименование на учебното заведение")]
        public string SchoolName { get; set; }

        [Display(Name = "Тип на учебното заведение")]
        public string SchoolType { get; set; }

        [Display(Name = "Финансиране")]
        public string SchoolTypeFinance { get; set; }

        [Display(Name = "Тип финансиране")]
        public string BudgetForm { get; set; }

        [Display(Name = "Булстат")]
        public string Bulstat { get; set; }

        [Display(Name = "Населено място")]
        public string City { get; set; }

        [Display(Name = "Област")]
        public string OblastName { get; set; }

        [Display(Name = "Община")]
        public string MunicipalityName { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Display(Name = "Пощенски код")]
        public string PostCode { get; set; }

        [Display(Name = "Телефон")]
        public string Tel { get; set; }

        [Display(Name = "Факс")]
        public string Fax { get; set; }

        [Display(Name = "Електронна поща")]
        public string Email { get; set; }

        public string SchlMidName { get; set; }

        public int BasicSchoolTypeID { get; set; }
    }
}
