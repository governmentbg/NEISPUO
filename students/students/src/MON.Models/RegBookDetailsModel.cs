namespace MON.Models
{
    using MON.Shared.Attributes;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    public class RegBookDetailsModel
    {
        [ExportIgnore]
        public int Id { get; set; }
        [ExportIgnore]
        [Display(Name = "Вид на документа")]
        public string BasicDocumentAbbr { get; set; }
        [Display(Name = "Вид на документа")]
        public string BasicDocumentName { get; set; }
        [Display(Name = "Учебна година")]
        public string SchoolYearName { get; set; }
        [Display(Name = "Име")]
        public string PersonFullName { get; set; }
        [Display(Name = "Идентификатор")]
        public string PersonalId { get; set; }
        [Display(Name = "Тип")]
        public string PersonalIdTypeName { get; set; }
        [Display(Name = "Серия")]
        public string Series { get; set; }
        [Display(Name = "Фабричен номер")]
        public string FactoryNumber { get; set; }
        [Display(Name = "Общ рег. №")]
        public string RegistrationNumberTotal { get; set; }
        [Display(Name ="Рег. № за годината")]
        public string RegistrationNumberYear { get; set; }
        [Display(Name = "Дата на регистрация")]
        public DateTime? RegistrationDate { get; set; }
        [Display(Name = "Форма на обучение")]
        public string EduForm { get; set; }
        [ExportIgnore]
        public int PersonId { get; set; }
        [ExportIgnore]
        public int BasicDocumentId { get; set; }
        [ExportIgnore]
        public int InstitutionId { get; set; }
        [ExportIgnore]
        [Display(Name = "Учебна година")]
        public short SchoolYear { get; set; }
        [ExportIgnore]
        [Display(Name = "Година на завършване")]
        public short YearGraduated { get; set; }
        [ExportIgnore]
        public int EduFormId { get; set; }
        [ExportIgnore]
        public string EducationSpecialization { get; set; }
        [ExportIgnore]
        [Display(Name = "Общ успех")]
        public string Gpa { get; set; }
        [ExportIgnore]
        public string IsAnulledStatus { get; set; }
        [Display(Name ="Анулиран")]
        public bool Canceled { get; set; }
        [ExportIgnore]
        public int ClassTypeId { get; set; }
        [ExportIgnore]
        public string Spacer { get; set; }
        [Display(Name ="Оригинал/Серия")]
        public string OriginalSeries { get; set; }
        [Display(Name = "Оригинал/Фабричен №")]
        public string OriginalFactoryNumber { get; set; }
        [Display(Name = "Оригинал/Общ. рег. №")]
        public string OriginalRegistrationNumber { get; set; }
        [Display(Name = "Оригинал/Рег. № за годината")]
        public string OriginalRegistrationNumberYear { get; set; }
        [Display(Name = "Оригинал/Рег. дата")]
        public DateTime? OriginalRegistrationDate { get; set; }
        [Display(Name = "Подпис на получателя")]
        public string Signed { get; set; }
    }
}
