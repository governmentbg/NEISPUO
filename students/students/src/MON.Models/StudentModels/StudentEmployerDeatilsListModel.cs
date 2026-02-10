namespace MON.Models.StudentModels
{
    using System;

    public class StudentEmployerDeatilsListModel
    {
        public string FullName { get; set; }
        public string Pin { get; set; }
        public string PinTypeName { get; set; }
        public short SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionCode { get; set; }
        public string InstitutionName { get; set; }
        public string BasicClassName { get; set; }
        public string ClassName { get; set; }
        public int Id { get; set; }
        public int BasicClassId { get; set; }
        public string InstitutionTown { get; set; }
        public string InstitutionMunicipality { get; set; }
        public string InstitutionRegion { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CompanyUic { get; set; }
        public string CompanyName { get; set; }

        /// <summary>
        /// Използва се ключ на грида
        /// </summary>
        public string Uid  => Guid.NewGuid().ToString();

        public int PersonId { get; set; }
        public string EduFormName { get; set; }
        public string Profession { get; set; }
    }
}
